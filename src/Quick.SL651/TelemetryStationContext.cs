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
        private byte[] buffer = new byte[1024];
        private byte[] buffer2 = new byte[8];

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
        /// 消息帧到达时
        /// </summary>
        public event EventHandler<UpgoingMessageFrame> MessageFrameArrived;

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
            try
            {
                var bufferStartIndex = 0;
                //读取包头
                bufferStartIndex += await readData(buffer, bufferStartIndex, 2, cancellationToken);
                //如果是HEX/BCD编码
                if (buffer.StartWith(UpgoingMessageFrame.HEX_BCD_S0H))
                {
                    FrameEncoding = FrameEncoding.HEX_BCD;
                }
                else if (buffer.StartWith(UpgoingMessageFrame.ASCII_S0H))
                {
                    FrameEncoding = FrameEncoding.ASCII;
                }
                else
                {
                    throw new IOException("未知包头：" + BitConverter.ToString(buffer, bufferStartIndex - 2, 2));
                }
                //读取中心站地址
                bufferStartIndex += await readData(FrameEncoding, buffer, bufferStartIndex, 1, cancellationToken);
                var centralStationAddress = buffer[bufferStartIndex - 1];
                //读取遥测站地址
                bufferStartIndex += await readData(FrameEncoding, buffer, bufferStartIndex, 5, cancellationToken);
                var telemetryStationAddress = new Span<byte>(buffer, bufferStartIndex - 5, 5).BCD_Decode();
                //读取密码
                bufferStartIndex += await readData(FrameEncoding, buffer, bufferStartIndex, 2, cancellationToken);
                var password = new byte[2];
                password[0] = buffer[bufferStartIndex - 2];
                password[1] = buffer[bufferStartIndex - 1];
                //读取功能码
                bufferStartIndex += await readData(FrameEncoding, buffer, bufferStartIndex, 1, cancellationToken);
                var functionCode = buffer[bufferStartIndex - 1];
                //读取报文上下行标识和报文长度
                bufferStartIndex += await readData(FrameEncoding, buffer, bufferStartIndex, 2, cancellationToken);
                buffer2[0] = buffer[bufferStartIndex - 2];
                buffer2[0] &= 0xF0;
                var isUpgoing = buffer2[0] == 0;
                if (!isUpgoing)
                    throw new IOException("预期接收上行报文，却收到了下行报文");
                buffer2[0] = buffer[bufferStartIndex - 2];
                buffer2[1] = buffer[bufferStartIndex - 1];
                buffer2[0] &= 0x0F;
                //如果CPU是小端字节序，则交换
                if (BitConverter.IsLittleEndian)
                {
                    buffer2[2] = buffer2[0];
                    buffer2[0] = buffer2[1];
                    buffer2[1] = buffer2[2];
                }
                var messageLength = BitConverter.ToInt16(buffer2);
                //读取报文开始符
                bufferStartIndex += await readData(FrameEncoding, buffer, bufferStartIndex, 1, cancellationToken);
                if (buffer[bufferStartIndex - 1] != UpgoingMessageFrame.STX)
                    throw new IOException($"意外的字符：0x{buffer[bufferStartIndex - 1].ToString("X2")}。预期字符：报文开始符(0x{UpgoingMessageFrame.STX.ToString("X2")})");
                //读取报文正文
                var messageStartIndex = bufferStartIndex;
                bufferStartIndex += await readData(FrameEncoding, buffer, bufferStartIndex, messageLength, cancellationToken);
                //读取报文结束符
                bufferStartIndex += await readData(FrameEncoding, buffer, bufferStartIndex, 1, cancellationToken);
                if (buffer[bufferStartIndex - 1] != UpgoingMessageFrame.ETX && buffer[bufferStartIndex - 1] != UpgoingMessageFrame.ETB)
                    throw new IOException($"意外的字符：0x{buffer[bufferStartIndex - 1].ToString("X2")}。预期字符：报文结束符(0x{UpgoingMessageFrame.ETX.ToString("X2")}或者0x{UpgoingMessageFrame.ETB.ToString("X2")})");
                //读取CRC校验值
                await readData(FrameEncoding, buffer2, 0, 2, cancellationToken);
                //校验CRC
                var crcResult = Crc16.Hash(new ReadOnlySpan<byte>(buffer, 0, bufferStartIndex));
                if (!buffer2.StartWith(crcResult))
                    throw new IOException("报文帧CRC检验失败！");

                //解析完成
                var messageFrame = new UpgoingMessageFrame(centralStationAddress,
                    telemetryStationAddress,
                    password,
                    functionCode,
                    new Memory<byte>(buffer, messageStartIndex, messageLength)
                    );
                //是否是第一次接收到消息
                var isFirstMessageFrameArrived = LastMessageFrame == null;
                LastMessageFrame = messageFrame;
                //第一次接收到消息时，触发连接已建立事件
                if (isFirstMessageFrameArrived)
                    Connected?.Invoke(this, EventArgs.Empty);
                //触发报文帧已到达事件
                MessageFrameArrived?.Invoke(this, messageFrame);
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

        private async Task<int> readData(FrameEncoding frameEncoding, byte[] buffer, int startIndex, int totalCount, CancellationToken cancellationToken)
        {
            var sourceTotalCount = totalCount;
            //如果是ASCII编码，读取的字节数翻倍
            if (frameEncoding == FrameEncoding.ASCII)
                totalCount *= 2;
            await readData(buffer, startIndex, totalCount, cancellationToken);
            //将ASCII编码转换为HEX/BCD编码
            if (frameEncoding == FrameEncoding.ASCII)
                for (var i = 0; i < totalCount; i += 2)
                {
                    var str = Encoding.ASCII.GetString(buffer, startIndex + i, 2);
                    var b = byte.Parse(str, System.Globalization.NumberStyles.HexNumber);
                    buffer[i / 2 + startIndex] = b;
                }
            return sourceTotalCount;
        }

        private async Task<int> readData(byte[] buffer, int startIndex, int totalCount, CancellationToken cancellationToken)
        {
            if (totalCount > buffer.Length - startIndex)
                throw new IOException($"Recv data length[{totalCount}] bigger than buffer length[{buffer.Length - startIndex}]");
            int ret;
            var count = 0;
            while (count < totalCount)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                var readTask = stream.ReadAsync(buffer, count + startIndex, totalCount - count, cancellationToken);
                ret = await await TaskUtils.TaskWait(readTask, centralStation.Options.TransportTimeout);
                if (readTask.IsCanceled || ret == 0)
                    break;
                if (ret < 0)
                    throw new IOException("Read error from stream.");
                count += ret;
            }
            return count;
        }
    }
}
