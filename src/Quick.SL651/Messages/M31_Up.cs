using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 均匀时段水文信息报上行报文
    /// </summary>
    public class M31_Up : AbstractMessage
    {
        public const byte FunctionCode = 0x31;

        public M31_Up(Memory<byte> t)
            : base(t)
        {

        }
    }
}
