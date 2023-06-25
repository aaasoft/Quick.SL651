﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 报文
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 流水号
        /// </summary>
        int SerialNumber { get; }
        /// <summary>
        /// 发报时间
        /// </summary>
        DateTime SendTime { get; }
        /// <summary>
        /// 报文结束符
        /// </summary>
        byte EndMark { get; set; }
        /// <summary>
        /// 报文结束符是否是ETX
        /// </summary>
        bool IsEndMarkETX { get; }
    }
}
