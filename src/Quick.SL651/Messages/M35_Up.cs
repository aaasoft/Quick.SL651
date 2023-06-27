using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 遥测站人工置数报上行报文
    /// </summary>
    public class M35_Up : AbstractMessage
    {
        public const byte FunctionCode = 0x35;

        public M35_Up(Memory<byte> t)
            : base(t)
        {

        }
    }
}
