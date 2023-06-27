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
        public M35_Up(Memory<byte> t)
            : base(t)
        {

        }
    }
}
