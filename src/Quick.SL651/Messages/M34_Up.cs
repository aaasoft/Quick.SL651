using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 遥测站小时报上行报文
    /// </summary>
    public class M34_Up : AbstractMessage
    {
        public M34_Up(Memory<byte> t)
            : base(t)
        {

        }
    }
}
