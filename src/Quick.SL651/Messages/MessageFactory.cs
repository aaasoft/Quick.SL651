using Quick.SL651.Utils;

namespace Quick.SL651.Messages
{
    public class MessageFactory
    {
        public static MessageFactory Instance { get; } = new MessageFactory();
        private Dictionary<byte, Func<Memory<byte>, IMessage>> upgoingMessageCreatorDict = new Dictionary<byte, Func<Memory<byte>, IMessage>>();
        public MessageFactory()
        {
            upgoingMessageCreatorDict[链路维持报.UpgoingMessage.FunctionCode] = t => new 链路维持报.UpgoingMessage(t);
            upgoingMessageCreatorDict[测试报.UpgoingMessage.FunctionCode] = t => new 测试报.UpgoingMessage(t);
            upgoingMessageCreatorDict[均匀时段水文信息报.UpgoingMessage.FunctionCode] = t => new 均匀时段水文信息报.UpgoingMessage(t);
            upgoingMessageCreatorDict[遥测站定时报.UpgoingMessage.FunctionCode] = t => new 遥测站定时报.UpgoingMessage(t);
            upgoingMessageCreatorDict[遥测站加报报.UpgoingMessage.FunctionCode] = t => new 遥测站加报报.UpgoingMessage(t);
            upgoingMessageCreatorDict[遥测站小时报.UpgoingMessage.FunctionCode] = t => new 遥测站小时报.UpgoingMessage(t);
        }

        public async Task<Tuple<int, IMessage>> ReadMessage(
            MessageFrameInfo frameInfo,
            Stream stream,
            byte[] read_buffer,
            int bufferStartIndex,
            CancellationToken cancellationToken,
            int readTimeout)
        {
            //读取报文正文
            var messageStartIndex = bufferStartIndex;
            bufferStartIndex += await TransportUtils.ReadData(frameInfo.FrameEncoding, stream, read_buffer, bufferStartIndex, frameInfo.MessageLength, cancellationToken, readTimeout);
            if (!upgoingMessageCreatorDict.ContainsKey(frameInfo.FunctionCode))
                throw new IOException($"无法解析报文。未知功能码：0x{frameInfo.FunctionCode.ToString("X2")}");
            var messageCreator = upgoingMessageCreatorDict[frameInfo.FunctionCode];
            var message = messageCreator.Invoke(new Memory<byte>(read_buffer, messageStartIndex, frameInfo.MessageLength));
            
            return new Tuple<int, IMessage>(bufferStartIndex, message);
        }
    }
}
