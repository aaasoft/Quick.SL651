using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 测试报上行报文
    /// </summary>
    public class M30_Up : AbstractMessageWithPointData
    {
        public M30_Up(Memory<byte> t)
            : base(t)
        {

        }
    }
}
