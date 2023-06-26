using System;
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
        public int TransportTimeout { get; set; } = 1 * 60 * 1000;
        public Func<TelemetryStationContext, WorkMode> GetTelemetryStationWorkModeFunc { get; set; } = t => WorkMode.M1;
    }
}
