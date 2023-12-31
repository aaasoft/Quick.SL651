﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Quick.SL651.Enums;

namespace Quick.SL651
{
    public class CentralStationOptions
    {
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }
        public int TransportTimeout { get; set; } = 5 * 60 * 1000;
        public WorkMode WorkMode { get; set; } = WorkMode.M2;
    }
}
