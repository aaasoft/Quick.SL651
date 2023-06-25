using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages.测试报
{
    public class UpgoingMessage : AbstractUpgoingMessage
    {
        public const byte FunctionCode = 0x30;

        public UpgoingMessage(Memory<byte> t)
            : base(t)
        {

        }
    }
}
