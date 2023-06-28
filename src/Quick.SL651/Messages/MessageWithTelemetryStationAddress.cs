using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    public class MessageWithTelemetryStationAddress : Message
    {

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

        //读取测站编码引导符
        private Span<byte> ReadST(Span<byte> span)
        {
            if (!span.StartsWith(ST))
                throw new IOException($"意外的字符：0x{span[0].ToString("X2")}-0x{span[1].ToString("X2")}。预期字符：测站编码引导符(0x{ST[0].ToString("X2")}或者0x{ST[1].ToString("X2")})");
            return span.Slice(ST.Length);
        }

        //读取遥测站地址
        private Span<byte> ReadTelemetryStationAddress(Span<byte> span)
        {
            TelemetryStationAddress = span.Slice(0, 5).ToArray();
            TelemetryStationAddress_Text = new Span<byte>(TelemetryStationAddress).BCD_Decode();
            return span.Slice(TelemetryStationAddress.Length);
        }

        public override Span<byte> Read(Span<byte> span)
        {
            span = base.Read(span);
            //读取测站编码引导符
            span = ReadST(span);
            //读取遥测站地址
            span = ReadTelemetryStationAddress(span);
            return span;
        }
    }
}
