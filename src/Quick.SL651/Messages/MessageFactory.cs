using Quick.SL651.Enums;
using Quick.SL651.Utils;

namespace Quick.SL651.Messages
{
    public class MessageFactory
    {
        public static MessageFactory Instance { get; } = new MessageFactory();
        private Dictionary<FunctionCodes, Func<IMessage>> upgoingMessageCreatorDict = new Dictionary<FunctionCodes, Func<IMessage>>();
        public MessageFactory()
        {
            upgoingMessageCreatorDict[FunctionCodes.M2F] = () => new Message();
            upgoingMessageCreatorDict[FunctionCodes.M30] = () => new MessageWithElementDatas();
            upgoingMessageCreatorDict[FunctionCodes.M31] = () => new MessageWithElementDatas();
            upgoingMessageCreatorDict[FunctionCodes.M32] = () => new MessageWithElementDatas();
            upgoingMessageCreatorDict[FunctionCodes.M33] = () => new MessageWithElementDatas();
            upgoingMessageCreatorDict[FunctionCodes.M34] = () => new MessageWithElementDatas();
            upgoingMessageCreatorDict[FunctionCodes.M37] = () => new MessageWithElementDatas();
            upgoingMessageCreatorDict[FunctionCodes.M3A] = () => new MessageWithElementDatas();
            upgoingMessageCreatorDict[FunctionCodes.M44] = () => new MessageWithElementDatas();
            upgoingMessageCreatorDict[FunctionCodes.M45] = () => new M_45_Up();
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
            var message = messageCreator.Invoke();
            message.Read(new Span<byte>(read_buffer, messageStartIndex, frameInfo.MessageLength));
            return new Tuple<int, IMessage>(bufferStartIndex, message);
        }
    }
}
