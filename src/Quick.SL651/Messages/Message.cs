using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    public class Message : IMessage
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public ushort SerialNumber { get; set; }
        /// <summary>
        /// 发报时间
        /// </summary>
        public DateTime SendTime { get; set; }

        public Message()
        {
            SerialNumber = 0;
            SendTime = DateTime.Now;
        }

        //读取流水号
        private Span<byte> ReadSerialNumber(Span<byte> span)
        {
            var serialNumberSpan = span.Slice(0, 2);
            if (BitConverter.IsLittleEndian)
                serialNumberSpan.Reverse();
            SerialNumber = BitConverter.ToUInt16(serialNumberSpan);
            if (BitConverter.IsLittleEndian)
                serialNumberSpan.Reverse();
            return span.Slice(serialNumberSpan.Length);
        }

        //读取发报时间
        private Span<byte> ReadSendTime(Span<byte> span)
        {
            var sendTimeSpan = span.Slice(0, 6);
            SendTime = ReadTimeFromBytes(sendTimeSpan);
            return span.Slice(sendTimeSpan.Length);
        }

        protected DateTime ReadTimeFromBytes(Span<byte> span)
        {
            var sendTimeStr = span.BCD_Decode();
            var nowTime = DateTime.Now;
            var yearPrefix = (nowTime.Year / 100).ToString();
            switch (span.Length)
            {
                case 5:
                    return DateTime.Parse($"{yearPrefix}{sendTimeStr.Substring(0, 2)}-{sendTimeStr.Substring(2, 2)}-{sendTimeStr.Substring(4, 2)} {sendTimeStr.Substring(6, 2)}:{sendTimeStr.Substring(8, 2)}:00");
                case 6:
                    return DateTime.Parse($"{yearPrefix}{sendTimeStr.Substring(0, 2)}-{sendTimeStr.Substring(2, 2)}-{sendTimeStr.Substring(4, 2)} {sendTimeStr.Substring(6, 2)}:{sendTimeStr.Substring(8, 2)}:{sendTimeStr.Substring(10, 2)}");
            }
            throw new IOException("解析时间失败，字节数组长度错误。");
        }

        public virtual Span<byte> Read(Span<byte> span)
        {
            //读取流水号
            span = ReadSerialNumber(span);
            //读取发报时间
            span = ReadSendTime(span);
            return span;
        }

        public virtual int Write(Stream stream)
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
