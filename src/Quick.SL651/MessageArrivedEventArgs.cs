using Quick.SL651.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651
{
    public class MessageArrivedEventArgs : EventArgs
    {
        /// <summary>
        /// 头
        /// </summary>
        public MessageFrameHead Head { get; set; }
        /// <summary>
        /// 是否是报文结束
        /// </summary>
        public bool IsETX { get; set; }
        public IUpgoingMessage UpgoingMessage { get; set; }
        public IDowngoingMessage DowngoingMessage { get; set; }
    }
}
