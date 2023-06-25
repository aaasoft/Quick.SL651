using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    public abstract class AbstractUpgoingMessage : IUpgoingMessage
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public int SerialNumber { get; private set; }
        /// <summary>
        /// 发报时间
        /// </summary>
        public DateTime SendTime { get; private set; }

        public AbstractUpgoingMessage(Memory<byte> t)
        {
            var serialNumberSpan = t.Slice(0, 2).Span;
            if (BitConverter.IsLittleEndian)
                serialNumberSpan.Reverse();
            SerialNumber = BitConverter.ToInt16(serialNumberSpan);

            var sendTimeSpan = t.Slice(2, 6).Span;
            var sendTimeStr = sendTimeSpan.BCD_Decode();
            var nowTime = DateTime.Now;
            var yearPrefix = (nowTime.Year / 100).ToString();
            SendTime = DateTime.Parse($"{yearPrefix}{sendTimeStr.Substring(0, 2)}-{sendTimeStr.Substring(2, 2)}-{sendTimeStr.Substring(4, 2)} {sendTimeStr.Substring(6, 2)}:{sendTimeStr.Substring(8, 2)}:{sendTimeStr.Substring(10, 2)}");
        }
    }
}
