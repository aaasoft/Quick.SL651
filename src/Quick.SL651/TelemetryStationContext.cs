using Quick.SL651.Enums;
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
        private TcpClient client;
        private NetworkStream stream;
        private int transportTimeout;
        private byte[] read_buffer = new byte[1024];
        private byte[] write_buffer = new byte[1024];

        /// <summary>
        /// 当前是否连接
        /// </summary>
        public bool IsConnected { get; private set; } = true;
        /// <summary>
        /// 遥测站信息
        /// </summary>
        public TelemetryStationInfo TelemetryStationInfo { get; private set; }
        /// <summary>
        /// 最后收到的消息
        /// </summary>
        public IMessage LastMessage { get; private set; }
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

        public TelemetryStationContext(TcpClient client, CancellationToken cancellationToken, int transportTimeout, WorkMode workMode)
        {
            this.client = client;
            RemoteEndPoint = client.Client.RemoteEndPoint;
            this.cancellationToken = cancellationToken;
            this.transportTimeout = transportTimeout;
            WorkMode = workMode;
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
                //读取帧头
                var messageFrameInfo = new MessageFrameInfo(WorkMode, true);
                bufferStartIndex = await messageFrameInfo.ReadHead(
                    stream,
                    read_buffer,
                    bufferStartIndex,
                    cancellationToken,
                    transportTimeout);

                //读取报文
                var bufferStartIndexAndMessage = await MessageFactory.Instance.ReadMessage(
                    messageFrameInfo,
                    stream,
                    read_buffer,
                    bufferStartIndex,
                    cancellationToken,
                    transportTimeout);
                bufferStartIndex = bufferStartIndexAndMessage.Item1;
                var message = bufferStartIndexAndMessage.Item2;

                //读取帧尾
                bufferStartIndex = await messageFrameInfo.ReadTail(
                    stream,
                    read_buffer,
                    bufferStartIndex,
                    cancellationToken,
                    transportTimeout);

                //是否是第一次接收到消息
                var isFirstMessageArrived = LastMessage == null;
                LastMessage = message;
                //第一次接收到消息时，触发连接已建立事件
                if (isFirstMessageArrived)
                {
                    TelemetryStationInfo = messageFrameInfo;
                    Connected?.Invoke(this, EventArgs.Empty);
                }
                var messageArrivedEventArgs = new MessageArrivedEventArgs()
                {
                    FrameInfo = messageFrameInfo,
                    Message = message
                };
                //触发报文帧已到达事件
                MessageFrameArrived?.Invoke(this, messageArrivedEventArgs);
                //如果工作模式不是M1
                if (WorkMode != WorkMode.M1)
                {
                    //确认消息
                    var confirmMessage = new M_Confirm_Down(message.SerialNumber, DateTime.Now);
                    switch (WorkMode)
                    {
                        case WorkMode.M2:
                        case WorkMode.M4:
                            if (messageFrameInfo.EndMark == EndMarks.ETB)
                                SendDowngoingMessage(messageFrameInfo.FunctionCode, confirmMessage, EndMarks.ACK);
                            else if (messageFrameInfo.EndMark == EndMarks.ETX)
                                SendDowngoingMessage(messageFrameInfo.FunctionCode, confirmMessage, EndMarks.ESC);
                            break;
                        case WorkMode.M3:
                            if (messageFrameInfo.EndMark == EndMarks.ETX)
                                SendDowngoingMessage(messageFrameInfo.FunctionCode, confirmMessage, EndMarks.ESC);
                            break;
                    }
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

        public void SendDowngoingMessage(FunctionCodes functionCode, IMessage message, EndMarks endMark)
        {
            lock (this)
                inner_SendDowngoingMessage(functionCode, message, endMark);
        }

        private void inner_SendDowngoingMessage(FunctionCodes functionCode, IMessage message, EndMarks endMark)
        {
            var ms = new MemoryStream(write_buffer);
            //写入帧起始符
            switch (TelemetryStationInfo.FrameEncoding)
            {
                case FrameEncoding.ASCII:
                    ms.Write(MessageFrameInfo.ASCII_S0H);
                    break;
                case FrameEncoding.HEX_BCD:
                    ms.Write(MessageFrameInfo.HEX_BCD_S0H);
                    break;
                default:
                    throw new IOException("未知帧编码：" + TelemetryStationInfo.FrameEncoding);
            }
            //写入遥测站地址
            ms.Write(TelemetryStationInfo.TelemetryStationAddress);
            //写入中心站地址
            ms.WriteByte(TelemetryStationInfo.CentralStationAddress);
            //写入密码
            ms.Write(TelemetryStationInfo.Password);
            //写入功能码
            ms.WriteByte((byte)functionCode);
            //写入报文上下行标识和长度
            var isUpgoingAndMessageLengthIndex = Convert.ToInt32(ms.Position);
            var isUpgoingAndMessageLengthSpan = new Span<byte>(write_buffer, isUpgoingAndMessageLengthIndex, 2);
            ms.WriteByte(0x80);
            ms.WriteByte(0x00);
            //写入报文开始标识
            ms.WriteByte((byte)StartMarks.STX);
            //写入报文
            var messageLength = Convert.ToUInt16(message.WriteTo(ms));
            //修改报文长度
            if (!BitConverter.TryWriteBytes(isUpgoingAndMessageLengthSpan, messageLength))
                throw new IOException($"写入报文长度[{messageLength}]时出错。");
            //如果是小端字节序
            if (BitConverter.IsLittleEndian)
                isUpgoingAndMessageLengthSpan.Reverse();
            var b = isUpgoingAndMessageLengthSpan[0];
            byte b_mask = 0xF0;
            b_mask ^= b;
            b = Convert.ToByte(b & b_mask);
            b = Convert.ToByte(b | 0x80);
            isUpgoingAndMessageLengthSpan[0] = b;

            //写入报文结束符
            ms.WriteByte((byte)endMark);
            ms.Flush();

            //计算CRC校验值
            var crcResult = Crc16.Hash(new ReadOnlySpan<byte>(write_buffer, 0, Convert.ToInt32(ms.Position)));
            //写入CRC校验值
            ms.Write(crcResult);
            ms.Flush();
            var messageFrameLength = Convert.ToInt32(ms.Position);
            ms.Close();

            switch (TelemetryStationInfo.FrameEncoding)
            {
                case FrameEncoding.ASCII:
                    var tmpBuffer = new byte[2];
                    for (var i = 0; i < messageFrameLength; i++)
                    {
                        b = write_buffer[i];
                        Encoding.ASCII.GetBytes(b.ToString("X2"), tmpBuffer);
                        stream.Write(tmpBuffer);
                    }
                    break;
                case FrameEncoding.HEX_BCD:
                    stream.Write(write_buffer, 0, messageFrameLength);
                    break;
            }
        }
    }
}
