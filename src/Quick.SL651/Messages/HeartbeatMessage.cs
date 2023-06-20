using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 链路维持报
    /// </summary>
    public class HeartbeatMessage : IMessage
    {
        public byte FunctionCode => 0x2F;
    }
}
