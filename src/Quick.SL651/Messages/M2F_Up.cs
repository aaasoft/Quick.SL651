using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 链路维持报上行报文
    /// </summary>
    public class M2F_Up : AbstractMessage
    {
        public const byte FunctionCode = 0x2F;
        public M2F_Up(Memory<byte> t)
            : base(t)
        {

        }
    }
}
