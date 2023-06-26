using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Enums
{
    /// <summary>
    /// 工作模式
    /// </summary>
    public enum WorkMode
    {
        /// <summary>
        /// 发送/无回答
        /// </summary>
        M1,
        /// <summary>
        /// 发送/确认
        /// </summary>
        M2,
        /// <summary>
        /// 多包发送/确认
        /// </summary>
        M3,
        /// <summary>
        /// 查询/响应
        /// </summary>
        M4
    }
}
