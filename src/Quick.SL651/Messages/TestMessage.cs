using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 测试报
    /// </summary>
    public class TestMessage : IMessage
    {
        public byte FunctionCode => 0x30;
    }
}
