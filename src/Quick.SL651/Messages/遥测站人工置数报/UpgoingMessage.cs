using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages.遥测站人工置数报
{
    public class UpgoingMessage : AbstractUpgoingMessage
    {
        public const byte FunctionCode = 0x35;

        public UpgoingMessage(Memory<byte> t)
            : base(t)
        {

        }
    }
}
