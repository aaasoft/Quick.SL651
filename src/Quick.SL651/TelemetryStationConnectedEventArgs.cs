using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651
{
    public class TelemetryStationConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// 遥测站上下文
        /// </summary>
        public TelemetryStationContext TelemetryStation { get; set; }
        /// <summary>
        /// 是否允许连接
        /// </summary>
        public bool Allowed { get; set; } = true;
        /// <summary>
        /// 当拒绝连接时返回的消息
        /// </summary>
        public string Message { get; set; }
    }
}
