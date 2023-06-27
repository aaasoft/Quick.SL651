using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Enums
{
    /// <summary>
    /// 遥测站分类码
    /// </summary>
    public enum TelemetryStationType : byte
    {
        /// <summary>
        /// 降水
        /// </summary>
        P = 0x50,
        /// <summary>
        /// 河道
        /// </summary>
        H = 0x48,
        /// <summary>
        /// 水库(湖泊)
        /// </summary>
        K = 0x4B,
        /// <summary>
        /// 闸坝
        /// </summary>
        Z = 0x5A,
        /// <summary>
        /// 泵站
        /// </summary>
        D = 0x44,
        /// <summary>
        /// 潮汐
        /// </summary>
        T = 0x54,
        /// <summary>
        /// 墒情
        /// </summary>
        M = 0x4D,
        /// <summary>
        /// 地下水
        /// </summary>
        G = 0x47,
        /// <summary>
        /// 水质
        /// </summary>
        Q = 0x51,
        /// <summary>
        /// 取水口
        /// </summary>
        I = 0x49,
        /// <summary>
        /// 排水口
        /// </summary>
        O = 0x4F
    }
}
