using Quick.SL651.Elements;
using Quick.SL651.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    public class MessageWithElementDatas : MessageWithTelemetryStationAddress
    {
        /// <summary>
        /// 观测时间引导符
        /// </summary>
        public readonly static byte[] TT = { 0xF0, 0xF0 };

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

        public override Span<byte> Read(Span<byte> span)
        {
            span = base.Read(span);
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
