using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 遥测站小时报
    /// </summary>
    public class HourMessage : IMessage
    {
        public byte FunctionCode => 0x33;
    }
}
