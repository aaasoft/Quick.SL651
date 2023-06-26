﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651.Messages
{
    /// <summary>
    /// 确认下行报文
    /// </summary>
    public class ConfirmDowngoingMessage : AbstractMessage
    {
        public ConfirmDowngoingMessage(ushort serialNumber, DateTime sendTime)
            : base(serialNumber, sendTime)
        {
        }
    }
}
