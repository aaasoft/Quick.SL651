using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.SL651.Enums;

namespace Quick.SL651
{
    public class TelemetryStationInfo
    {
        /// <summary>
        /// 帧编码
        /// </summary>
        public FrameEncoding FrameEncoding { get; set; }
        /// <summary>
        /// 中心站地址
        /// </summary>
        public byte CentralStationAddress { get; set; }
        /// <summary>
        /// 遥测站地址
        /// </summary>
        public byte[] TelemetryStationAddress { get; set; }
        /// <summary>
        /// 遥测站地址文本形式
        /// </summary>
        public string TelemetryStationAddress_Text { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public byte[] Password { get; set; }
    }
}
