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
        /// 报文
        /// </summary>
        public IMessage Message { get; set; }
    }
}
