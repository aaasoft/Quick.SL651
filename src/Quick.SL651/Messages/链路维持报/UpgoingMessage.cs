using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages.链路维持报
{
    public class UpgoingMessage : AbstractMessage
    {
        public const byte FunctionCode = 0x2F;
        public UpgoingMessage(Memory<byte> t)
            : base(t)
        {

        }
    }
}
