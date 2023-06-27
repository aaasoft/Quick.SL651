using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 遥测站定时报上行报文
    /// </summary>
    public class M_32_Up : AbstractMessageWithPointData
    {
        public M_32_Up(Memory<byte> t)
            : base(t)
        {

        }
    }
}
