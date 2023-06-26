using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    public abstract class AbstractMessage : IMessage
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public ushort SerialNumber { get; private set; }
        /// <summary>
        /// 发报时间
        /// </summary>
        public DateTime SendTime { get; private set; }

        public AbstractMessage(ushort serialNumber, DateTime sendTime)
        {
            SerialNumber = serialNumber;
            SendTime = sendTime;
        }

        public AbstractMessage(Memory<byte> memory)
        {
            var serialNumberSpan = new Span<byte>(memory.Slice(0, 2).ToArray());
            if (BitConverter.IsLittleEndian)
                serialNumberSpan.Reverse();
            SerialNumber = BitConverter.ToUInt16(serialNumberSpan);

            var sendTimeSpan = memory.Slice(2, 6).Span;
            var sendTimeStr = sendTimeSpan.BCD_Decode();
            var nowTime = DateTime.Now;
            var yearPrefix = (nowTime.Year / 100).ToString();
            SendTime = DateTime.Parse($"{yearPrefix}{sendTimeStr.Substring(0, 2)}-{sendTimeStr.Substring(2, 2)}-{sendTimeStr.Substring(4, 2)} {sendTimeStr.Substring(6, 2)}:{sendTimeStr.Substring(8, 2)}:{sendTimeStr.Substring(10, 2)}");
        }

        public int WriteTo(Stream stream)
        {
            var count = 0;
            var buffer = BitConverter.GetBytes(SerialNumber);
            if (BitConverter.IsLittleEndian)
                new Span<byte>(buffer).Reverse();
            stream.Write(buffer);
            count += buffer.Length;

            var nowTime = DateTime.Now;
            var numberArray = new int[]
            {
                nowTime.Year %100, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, nowTime.Second
            };
            foreach (var number in numberArray)
            {
                var b = byte.Parse(number.ToString("D2"), System.Globalization.NumberStyles.HexNumber);
                stream.WriteByte(b);
                count++;
            }
            return count;
        }
    }
}
