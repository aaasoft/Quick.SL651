using Quick.SL651.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    public abstract class AbstractMessageWithPointData : AbstractMessage
    {
        /// <summary>
        /// 观测时间引导符
        /// </summary>
        public readonly static byte[] TT = { 0xF0, 0xF0 };
        /// <summary>
        /// 测站编码引导符
        /// </summary>
        public readonly static byte[] ST = { 0xF1, 0xF1 };

        /// <summary>
        /// 遥测站地址
        /// </summary>
        public byte[] TelemetryStationAddress { get; set; }
        /// <summary>
        /// 遥测站地址文本形式
        /// </summary>
        public string TelemetryStationAddress_Text { get; set; }
        /// <summary>
        /// 遥测站分类
        /// </summary>
        public TelemetryStationType TelemetryStationType { get; set; }

        protected AbstractMessageWithPointData(Memory<byte> memory) : base(memory)
        {
            var span = memory.Span;
            //读取测站编码引导符
            if (!span.StartsWith(ST))
                throw new IOException($"意外的字符：0x{span[0].ToString("X2")}-0x{span[1].ToString("X2")}。预期字符：测站编码引导符(0x{ST[0].ToString("X2")}或者0x{ST[1].ToString("X2")})");
            span = span.Slice(ST.Length);
            //读取遥测站地址
            TelemetryStationAddress = span.Slice(0, 5).ToArray();
            TelemetryStationAddress_Text = new Span<byte>(TelemetryStationAddress).BCD_Decode();
            span = span.Slice(TelemetryStationAddress.Length);
            //读取遥测站分类码
            TelemetryStationType = (TelemetryStationType)span[0];
            span = span.Slice(1);
        }

        protected AbstractMessageWithPointData(ushort serialNumber, DateTime sendTime) : base(serialNumber, sendTime)
        {
        }
    }
}
