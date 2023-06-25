using Quick.SL651.Messages;

namespace Quick.SL651
{
    public class UpgoingMessageFrame
    {
        /// <summary>
        /// 帧头
        /// </summary>
        public MessageFrameHead Head { get; private set; }

        /// <summary>
        /// 报文内容
        /// </summary>
        public IUpgoingMessage Message { get; private set; }

        public UpgoingMessageFrame(
            MessageFrameHead head,
            Memory<byte> body)
        {
            Head = head;
            //解析报文
            Message = MessageFactory.Instance.ParseUpgoingMessage(head.FunctionCode, body);
        }
    }
}
