using Quick.SL651.Enums;
using Quick.SL651.Utils;

namespace Quick.SL651.Messages
{
    public class MessageFactory
    {
        public static MessageFactory Instance { get; } = new MessageFactory();
        private Dictionary<FunctionCodes, Func<Memory<byte>, IMessage>> upgoingMessageCreatorDict = new Dictionary<FunctionCodes, Func<Memory<byte>, IMessage>>();
        public MessageFactory()
        {
            upgoingMessageCreatorDict[FunctionCodes.M2F] = t => new M2F_Up(t);
            upgoingMessageCreatorDict[FunctionCodes.M30] = t => new M30_Up(t);
            upgoingMessageCreatorDict[FunctionCodes.M31] = t => new M31_Up(t);
            upgoingMessageCreatorDict[FunctionCodes.M32] = t => new M32_Up(t);
            upgoingMessageCreatorDict[FunctionCodes.M33] = t => new M33_Up(t);
            upgoingMessageCreatorDict[FunctionCodes.M34] = t => new M34_Up(t);
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
