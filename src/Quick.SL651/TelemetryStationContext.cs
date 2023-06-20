using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651
{
    public class TelemetryStationContext
    {
        private CancellationToken cancellationToken;
        private TcpClient client;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];
        /// <summary>
        /// 当前是否连接
        /// </summary>
        public bool IsConnected { get; private set; } = true;
        /// <summary>
        /// 帧编码
        /// </summary>
        public FrameEncoding FrameEncoding { get; private set; } = FrameEncoding.Unknown;
        /// <summary>
        /// 新报文帧到达时
        /// </summary>
        public event EventHandler<MessageFrame> NewMessageFrameArrived;

        public TelemetryStationContext(TcpClient client, CancellationToken cancellationToken)
        {
            this.client = client;
            this.cancellationToken = cancellationToken;
        }

        internal void Start()
        {
            stream = client.GetStream();
            beginReadData();
        }

        private void onError(string error)
        {
            IsConnected = false;
        }

        private void beginReadData()
        {
            if (!IsConnected || cancellationToken.IsCancellationRequested)
                return;
            stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;
                if (t.IsFaulted)
                {
                    onError(t.Exception.InnerException.Message);
                    return;
                }
                var ret = t.Result;
                if (ret <= 0)
                {
                    onError("接收到的数据为空");
                    return;
                }
                try
                {
                    //解析报文
                    handleData(new Span<byte>(buffer, 0, ret));
                }
                catch (Exception ex)
                {
                    onError(ex.Message);
                    return;
                }
                //继续读下一条数据
                beginReadData();
            });
        }

        private void handleData(Span<byte> data)
        {
            //是否是第一次接收数据
            var isFirstTime = FrameEncoding == FrameEncoding.Unknown;
            //如果是第一次接收数据，判断帧编码
            if (isFirstTime)
            {
                if (data.StartsWith(MessageFrame.HEX_BCD_S0H))
                    FrameEncoding = FrameEncoding.HEX_BCD;
                else if (data.StartsWith(MessageFrame.ASCII_S0H))
                    FrameEncoding = FrameEncoding.ASCII;
                else
                    throw new IOException("未知包头。" + BitConverter.ToString(data.ToArray()));
            }
            var messageFrame = MessageFrame.Parse(FrameEncoding, data);
            NewMessageFrameArrived?.Invoke(this, messageFrame);
        }
    }
}
