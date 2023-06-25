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
        public MessageFrameHead Head { get; set; }
        public IUpgoingMessage UpgoingMessage { get; set; }
        public IDowngoingMessage DowngoingMessage { get; set; }
    }
}
