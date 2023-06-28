using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 中心站查询遥测站软件版本上行报文
    /// </summary>
    public class M_45_Up : MessageWithTelemetryStationAddress
    {
        public string TelemetryStationVersion { get; set; }
        public override Span<byte> Read(Span<byte> span)
        {
            span = base.Read(span);
            var byteCount = span[0];
            span = span.Slice(1);
            TelemetryStationVersion = Encoding.ASCII.GetString(span.Slice(0, byteCount));
            span = span.Slice(byteCount);
            return span;
        }
    }
}
