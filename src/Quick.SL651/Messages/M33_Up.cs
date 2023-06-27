using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 遥测站加报报上行报文
    /// </summary>
    public class M33_Up : AbstractMessageWithPointData
    {
        public M33_Up(Memory<byte> memory)
            : base(memory)
        {

        }
    }
}
