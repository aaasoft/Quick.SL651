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
        /// 帧信息
        /// </summary>
        public MessageFrameInfo FrameInfo { get; set; }
        /// <summary>
        /// 上行报文
        /// </summary>
        public IMessage UpgoingMessage { get; set; }
        /// <summary>
        /// 下行报文
        /// </summary>
        public IMessage DowngoingMessage { get; set; }
    }
}
