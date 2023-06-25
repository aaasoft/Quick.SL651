﻿using System;
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
        public int SerialNumber { get; private set; }
        /// <summary>
        /// 发报时间
        /// </summary>
        public DateTime SendTime { get; private set; }
        /// <summary>
        /// 报文结束符
        /// </summary>
        public byte EndMark { get; set; }
        /// <summary>
        /// 报文结束符是否是ETX
        /// </summary>
        public bool IsEndMarkETX => EndMark == MessageFrameHead.ETX;

        public AbstractMessage(int serialNumber, DateTime sendTime)
            : this(serialNumber, sendTime, MessageFrameHead.ETX)
        {
        }

        public AbstractMessage(int serialNumber, DateTime sendTime, byte endMark)
        {
            SerialNumber = serialNumber;
            SendTime = sendTime;
            EndMark = endMark;
        }

        public AbstractMessage(Memory<byte> memory)
        {
            var serialNumberSpan = new Span<byte>(memory.Slice(0, 2).ToArray());
            if (BitConverter.IsLittleEndian)
                serialNumberSpan.Reverse();
            SerialNumber = BitConverter.ToInt16(serialNumberSpan);

            var sendTimeSpan = memory.Slice(2, 6).Span;
            var sendTimeStr = sendTimeSpan.BCD_Decode();
            var nowTime = DateTime.Now;
            var yearPrefix = (nowTime.Year / 100).ToString();
            SendTime = DateTime.Parse($"{yearPrefix}{sendTimeStr.Substring(0, 2)}-{sendTimeStr.Substring(2, 2)}-{sendTimeStr.Substring(4, 2)} {sendTimeStr.Substring(6, 2)}:{sendTimeStr.Substring(8, 2)}:{sendTimeStr.Substring(10, 2)}");
        }
    }
}