using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Enums
{
    public enum StartMarks : byte
    {
        /// <summary>
        /// 传输正文起始
        /// </summary>
        STX = 0x02,
        /// <summary>
        /// 多包传输正文起始
        /// </summary>
        SYN = 0x16
    }
}
