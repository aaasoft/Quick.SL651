using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages.遥测站人工置数报
{
    public class DowngoingMessage : AbstractMessage
    {
        public DowngoingMessage(int serialNumber, DateTime sendTime) : base(serialNumber, sendTime)
        {
        }
    }
}
