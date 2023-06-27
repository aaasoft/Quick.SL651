using Quick.SL651.Elements;
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
        /// <summary>
        /// 观测时间
        /// </summary>
        public DateTime ObservedTime { get; set; }
        /// <summary>
        /// 要素值数组
        /// </summary>
        public ElementData[] ElementDatas { get; set; }

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

        //读取遥测站分类码
        private Span<byte> ReadTelemetryStationType(Span<byte> span)
        {
            TelemetryStationType = (TelemetryStationType)span[0];
            return span.Slice(1);
        }

        //读取观测时间引导符
        private Span<byte> ReadTT(Span<byte> span)
        {
            if (!span.StartsWith(TT))
                throw new IOException($"意外的字符：0x{span[0].ToString("X2")}-0x{span[1].ToString("X2")}。预期字符：观测时间引导符(0x{TT[0].ToString("X2")}或者0x{TT[1].ToString("X2")})");
            return span.Slice(TT.Length);
        }

        //读取观测时间
        private Span<byte> ReadObservedTime(Span<byte> span)
        {
            var sendTimeSpan = span.Slice(0, 5);
            ObservedTime = ReadTimeFromBytes(sendTimeSpan);
            return span.Slice(sendTimeSpan.Length);
        }

        protected AbstractMessageWithPointData(Memory<byte> memory) : base(memory)
        {
        }

        protected AbstractMessageWithPointData(ushort serialNumber, DateTime sendTime) : base(serialNumber, sendTime)
        {
        }


        protected override Span<byte> Load(Span<byte> span)
        {
            span = base.Load(span);
            //读取测站编码引导符
            span = ReadST(span);
            //读取遥测站地址
            span = ReadTelemetryStationAddress(span);
            //读取遥测站分类码
            span = ReadTelemetryStationType(span);
            //读取观测时间引导符
            span = ReadTT(span);
            //读取观测时间
            span = ReadObservedTime(span);
            //读取要素
            List<ElementData> elementDataList = new List<ElementData>();
            while (span.Length > 0)
            {
                var elementData = new ElementData();
                span = elementData.Load(span);
                elementDataList.Add(elementData);
            }
            ElementDatas = elementDataList.ToArray();
            return span;
        }
    }
}
