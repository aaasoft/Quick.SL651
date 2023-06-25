using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    public class MessageFactory
    {
        public static MessageFactory Instance { get; } = new MessageFactory();
        private Dictionary<byte, Func<Memory<byte>, IUpgoingMessage>> upgoingMessageCreatorDict = new Dictionary<byte, Func<Memory<byte>, IUpgoingMessage>>();
        public MessageFactory()
        {
            upgoingMessageCreatorDict[链路维持报.UpgoingMessage.FunctionCode] = t => new 链路维持报.UpgoingMessage(t);
            upgoingMessageCreatorDict[测试报.UpgoingMessage.FunctionCode] = t => new 测试报.UpgoingMessage(t);
            upgoingMessageCreatorDict[均匀时段水文信息报.UpgoingMessage.FunctionCode] = t => new 均匀时段水文信息报.UpgoingMessage(t);
            upgoingMessageCreatorDict[遥测站定时报.UpgoingMessage.FunctionCode] = t => new 遥测站定时报.UpgoingMessage(t);
            upgoingMessageCreatorDict[遥测站加报报.UpgoingMessage.FunctionCode] = t => new 遥测站加报报.UpgoingMessage(t);
            upgoingMessageCreatorDict[遥测站小时报.UpgoingMessage.FunctionCode] = t => new 遥测站小时报.UpgoingMessage(t);
        }

        public IUpgoingMessage ParseUpgoingMessage(byte functionCode, Memory<byte> memory)
        {
            if (!upgoingMessageCreatorDict.ContainsKey(functionCode))
                throw new IOException($"无法解析报文。未知功能码：0x{functionCode.ToString("X2")}");
            var messageCreator = upgoingMessageCreatorDict[functionCode];
            return messageCreator.Invoke(memory);
        }
    }
}
