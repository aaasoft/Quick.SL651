using Quick.SL651.Messages;
using Quick.SL651.Utils;
using System.IO;
using System.IO.Hashing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Quick.SL651
{
    public class TelemetryStationContext
    {
        private CancellationToken cancellationToken;
        private CentralStation centralStation;
        private TcpClient client;
        private NetworkStream stream;
        private byte[] read_buffer = new byte[1024];
        private byte[] read_buffer2 = new byte[8];
        private byte[] write_buffer = new byte[1024];

        /// <summary>
        /// 当前是否连接
        /// </summary>
        public bool IsConnected { get; private set; } = true;
        /// <summary>
        /// 帧编码
        /// </summary>
        public FrameEncoding FrameEncoding { get; private set; } = FrameEncoding.Unknown;
        /// <summary>
        /// 最后收到的消息帧
        /// </summary>
        public UpgoingMessageFrame LastMessageFrame { get; private set; }
        /// <summary>
        /// 连接建立时
        /// </summary>
        public event EventHandler Connected;
        /// <summary>
        /// 连接断开时
        /// </summary>
        public event EventHandler<Exception> Disconnected;
        /// <summary>
        /// 远程端点
        /// </summary>
        public EndPoint RemoteEndPoint { get; private set; }
        /// <summary>
        /// 工作模式
        /// </summary>
        public WorkMode WorkMode { get; private set; }

        /// <summary>
        /// 消息帧到达时
        /// </summary>
        public event EventHandler<MessageArrivedEventArgs> MessageFrameArrived;

        public TelemetryStationContext(CentralStation centralStation, TcpClient client, CancellationToken cancellationToken)
        {
            this.centralStation = centralStation;
            this.client = client;
            RemoteEndPoint = client.Client.RemoteEndPoint;
            this.cancellationToken = cancellationToken;
        }

        internal void Start()
        {
            stream = client.GetStream();
            _ = beginReadData();
        }

        private void onError(Exception ex)
        {
            stream?.Dispose();
            stream = null;
            client?.Dispose();
            client = null;
            IsConnected = false;
            Disconnected?.Invoke(this, ex);
        }

        private async Task beginReadData()
        {
            if (!IsConnected || cancellationToken.IsCancellationRequested)
                return;
            var readTimeout = centralStation.Options.TransportTimeout;
            try
            {
                var bufferStartIndex = 0;
                //读取包头
                bufferStartIndex += await TransportUtils.ReadData(stream, read_buffer, bufferStartIndex, 2, cancellationToken, readTimeout);
                //如果是HEX/BCD编码
                if (read_buffer.StartWith(MessageFrameHead.HEX_BCD_S0H))
                {
                    FrameEncoding = FrameEncoding.HEX_BCD;
                }
                else if (read_buffer.StartWith(MessageFrameHead.ASCII_S0H))
                {
                    FrameEncoding = FrameEncoding.ASCII;
                }
                else
                {
                    throw new IOException("未知包头：" + BitConverter.ToString(read_buffer, bufferStartIndex - 2, 2));
                }
                //读取中心站地址
                bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
                var centralStationAddress = read_buffer[bufferStartIndex - 1];
                //读取遥测站地址
                bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 5, cancellationToken, readTimeout);
                var telemetryStationAddress = new Span<byte>(read_buffer, bufferStartIndex - 5, 5).BCD_Decode();
                //读取密码
                bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 2, cancellationToken, readTimeout);
                var password = new byte[2];
                password[0] = read_buffer[bufferStartIndex - 2];
                password[1] = read_buffer[bufferStartIndex - 1];
                //读取功能码
                bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
                var functionCode = read_buffer[bufferStartIndex - 1];
                //读取报文上下行标识和报文长度
                bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 2, cancellationToken, readTimeout);
                read_buffer2[0] = read_buffer[bufferStartIndex - 2];
                read_buffer2[0] &= 0xF0;
                var isUpgoing = read_buffer2[0] == 0;
                if (!isUpgoing)
                    throw new IOException("预期接收上行报文，却收到了下行报文");
                read_buffer2[0] = read_buffer[bufferStartIndex - 2];
                read_buffer2[1] = read_buffer[bufferStartIndex - 1];
                read_buffer2[0] &= 0x0F;
                //如果CPU是小端字节序，则交换
                if (BitConverter.IsLittleEndian)
                {
                    read_buffer2[2] = read_buffer2[0];
                    read_buffer2[0] = read_buffer2[1];
                    read_buffer2[1] = read_buffer2[2];
                }
                var messageLength = BitConverter.ToInt16(read_buffer2);
                //读取报文开始符
                bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
                if (read_buffer[bufferStartIndex - 1] != MessageFrameHead.STX)
                    throw new IOException($"意外的字符：0x{read_buffer[bufferStartIndex - 1].ToString("X2")}。预期字符：报文开始符(0x{MessageFrameHead.STX.ToString("X2")})");
                //读取报文正文
                var messageStartIndex = bufferStartIndex;
                bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, messageLength, cancellationToken, readTimeout);
                //读取报文结束符
                bufferStartIndex += await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
                //报文结束符
                var messageEndByte = read_buffer[bufferStartIndex - 1];
                if (messageEndByte != MessageFrameHead.ETX && messageEndByte != MessageFrameHead.ETB)
                    throw new IOException($"意外的字符：0x{messageEndByte.ToString("X2")}。预期字符：报文结束符(0x{MessageFrameHead.ETX.ToString("X2")}或者0x{MessageFrameHead.ETB.ToString("X2")})");
                //读取CRC校验值
                await TransportUtils.ReadData(FrameEncoding, stream, read_buffer2, 0, 2, cancellationToken, readTimeout);
                //校验CRC
                var crcResult = Crc16.Hash(new ReadOnlySpan<byte>(read_buffer, 0, bufferStartIndex));
                if (!read_buffer2.StartWith(crcResult))
                    throw new IOException("报文帧CRC检验失败！");
                //帧头
                var messageHead = new MessageFrameHead(
                        centralStationAddress,
                        telemetryStationAddress,
                        password,
                        functionCode,
                        isUpgoing,
                        messageLength
                    );
                //解析完成
                var messageFrame = new UpgoingMessageFrame(messageHead,
                    new Memory<byte>(read_buffer, messageStartIndex, messageLength));
                //是否是第一次接收到消息
                var isFirstMessageFrameArrived = LastMessageFrame == null;
                LastMessageFrame = messageFrame;
                //第一次接收到消息时，触发连接已建立事件
                if (isFirstMessageFrameArrived)
                {
                    WorkMode = centralStation.Options.GetTelemetryStationWorkModeFunc(this);
                    Connected?.Invoke(this, EventArgs.Empty);
                }
                var messageArrivedEventArgs = new MessageArrivedEventArgs()
                {
                    Head = messageHead,
                    UpgoingMessage = messageFrame.Message,
                    IsETX = messageEndByte == MessageFrameHead.ETX
                };
                //触发报文帧已到达事件
                MessageFrameArrived?.Invoke(this, messageArrivedEventArgs);

                //如果结束符是ETX，且工作模式是M2或者M3，且设置了下行消息，则回复确认收到
                if (messageEndByte == MessageFrameHead.ETX
                    && (WorkMode == WorkMode.M2 || WorkMode == WorkMode.M3)
                    && messageArrivedEventArgs.DowngoingMessage != null)
                {
                    //stream.Write(write_buffer);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                onError(ex);
                return;
            }
            _ = beginReadData();
        }
    }
}
