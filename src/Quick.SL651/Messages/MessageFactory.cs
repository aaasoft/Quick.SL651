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
            MessageFrameHead messageFrameHead,
            Stream stream,
            byte[] read_buffer,
            int bufferStartIndex,
            CancellationToken cancellationToken,
            int readTimeout)
        {
            //读取报文开始符
            bufferStartIndex += await TransportUtils.ReadData(messageFrameHead.FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
            if (read_buffer[bufferStartIndex - 1] != MessageFrameHead.STX)
                throw new IOException($"意外的字符：0x{read_buffer[bufferStartIndex - 1].ToString("X2")}。预期字符：报文开始符(0x{MessageFrameHead.STX.ToString("X2")})");
            //读取报文正文
            var messageStartIndex = bufferStartIndex;
            bufferStartIndex += await TransportUtils.ReadData(messageFrameHead.FrameEncoding, stream, read_buffer, bufferStartIndex, messageFrameHead.MessageLength, cancellationToken, readTimeout);
            //读取报文结束符
            bufferStartIndex += await TransportUtils.ReadData(messageFrameHead.FrameEncoding, stream, read_buffer, bufferStartIndex, 1, cancellationToken, readTimeout);
            //报文结束符
            var messageEndMark = read_buffer[bufferStartIndex - 1];
            if (messageEndMark != MessageFrameHead.ETX && messageEndMark != MessageFrameHead.ETB)
                throw new IOException($"意外的字符：0x{messageEndMark.ToString("X2")}。预期字符：报文结束符(0x{MessageFrameHead.ETX.ToString("X2")}或者0x{MessageFrameHead.ETB.ToString("X2")})");

            if (!upgoingMessageCreatorDict.ContainsKey(messageFrameHead.FunctionCode))
                throw new IOException($"无法解析报文。未知功能码：0x{messageFrameHead.FunctionCode.ToString("X2")}");
            var messageCreator = upgoingMessageCreatorDict[messageFrameHead.FunctionCode];
            var message = messageCreator.Invoke(new Memory<byte>(read_buffer, messageStartIndex, messageFrameHead.MessageLength));
            message.EndMark = messageEndMark;
            return new Tuple<int, IMessage>(bufferStartIndex, message);
        }
    }
}
