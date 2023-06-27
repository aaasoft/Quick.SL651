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
    public class M_2F_Up : AbstractMessage
    {
        public M_2F_Up(Memory<byte> t)
            : base(t)
        {

        }
    }
}
