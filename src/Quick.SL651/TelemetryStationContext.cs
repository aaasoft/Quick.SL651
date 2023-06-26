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
        /// 帧编码
        /// </summary>
        public FrameEncoding FrameEncoding { get; private set; } = FrameEncoding.Unknown;
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
                //读取帧头
                var messageFrameHead = new MessageFrameHead();
                var bufferStartIndex = 0;
                bufferStartIndex = await messageFrameHead.Read(
                    true,
                    stream,
                    read_buffer,
                    bufferStartIndex,
                    cancellationToken,
                    readTimeout);

                //读取报文
                var bufferStartIndexAndMessage = await MessageFactory.Instance.ReadMessage(
                    messageFrameHead,
                    stream,
                    read_buffer,
                    bufferStartIndex,
                    cancellationToken,
                    readTimeout);
                bufferStartIndex = bufferStartIndexAndMessage.Item1;
                var message = bufferStartIndexAndMessage.Item2;
                //读取CRC校验值
                await TransportUtils.ReadData(FrameEncoding, stream, read_buffer, bufferStartIndex, 2, cancellationToken, readTimeout);
                //校验CRC
                var crcResult = Crc16.Hash(new ReadOnlySpan<byte>(read_buffer, 0, bufferStartIndex));
                if (read_buffer[bufferStartIndex] != crcResult[0]
                    || read_buffer[bufferStartIndex + 1] != crcResult[1])
                    throw new IOException("报文帧CRC检验失败！");

                //是否是第一次接收到消息
                var isFirstMessageArrived = LastMessage == null;
                LastMessage = message;
                //第一次接收到消息时，触发连接已建立事件
                if (isFirstMessageArrived)
                {
                    WorkMode = centralStation.Options.GetTelemetryStationWorkModeFunc(this);
                    TelemetryStationInfo = messageFrameHead;
                    Connected?.Invoke(this, EventArgs.Empty);
                }
                var messageArrivedEventArgs = new MessageArrivedEventArgs()
                {
                    Head = messageFrameHead,
                    UpgoingMessage = message
                };
                //触发报文帧已到达事件
                MessageFrameArrived?.Invoke(this, messageArrivedEventArgs);

                //如果结束符是ETX，且工作模式是M2或者M3，且设置了下行消息，则回复确认收到
                if (message.IsEndMarkETX
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
