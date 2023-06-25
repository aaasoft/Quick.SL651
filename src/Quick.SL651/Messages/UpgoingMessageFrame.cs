using Quick.SL651.Messages;
using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651
{
    public class UpgoingMessageFrame : AbstractMessageFrame
    {
        public override bool IsUpgoing => true;
        /// <summary>
        /// 报文内容
        /// </summary>
        public IUpgoingMessage Message { get; private set; }

        public UpgoingMessageFrame(
            byte centralStationAddress,
            string telemetryStationAddress,
            byte[] password,
            byte functionCode,
            Memory<byte> messageData)
        {
            CentralStationAddress = centralStationAddress;
            TelemetryStationAddress = telemetryStationAddress;
            Password = password;
            FunctionCode = functionCode;
            MessageLength = messageData.Length;
            //解析报文
            Message = MessageFactory.Instance.ParseUpgoingMessage(functionCode, messageData);
        }
    }
}
