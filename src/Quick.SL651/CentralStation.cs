using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Quick.SL651
{
    public class CentralStation
    {
        public CentralStationOptions Options { get; private set; }
        private TcpListener tcpListener;
        private CancellationTokenSource cts;
        /// <summary>
        /// 遥测站已连接事件
        /// </summary>
        public event EventHandler<TelemetryStationConnectedEventArgs> TelemetryStationConnected;
        private List<TelemetryStationContext> telemetryStationList = new List<TelemetryStationContext>();

        public CentralStation(CentralStationOptions options)
        {
            this.Options = options;
        }

        public void Start()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            tcpListener = new TcpListener(Options.IPAddress, Options.Port);
            tcpListener.Start();
            beginAccept(cts.Token);
        }

        private void beginAccept(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                TcpClient client = null;
                try
                {
                    client = await tcpListener.AcceptTcpClientAsync(cancellationToken);
                    beginAccept(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                var telemetryStation = new TelemetryStationContext(client, cancellationToken, Options.TransportTimeout, Options.WorkMode);
                lock (telemetryStationList)
                    telemetryStationList.Add(telemetryStation);
                telemetryStation.Connected += TelemetryStation_Connected;
                telemetryStation.Disconnected += TelemetryStation_Disconnected;
                telemetryStation.Start();
            });
        }

        private void TelemetryStation_Disconnected(object sender, Exception e)
        {
            var telemetryStation = (TelemetryStationContext)sender;
            telemetryStation.Connected -= TelemetryStation_Connected;
            telemetryStation.Disconnected -= TelemetryStation_Disconnected;
            lock (telemetryStationList)
                telemetryStationList.Remove(telemetryStation);
        }

        private void TelemetryStation_Connected(object sender, EventArgs e)
        {
            var telemetryStation = (TelemetryStationContext)sender;
            var telemetryStationConnectedEventArgs = new TelemetryStationConnectedEventArgs()
            {
                TelemetryStation = telemetryStation
            };
            TelemetryStationConnected?.Invoke(this, telemetryStationConnectedEventArgs);
            //如果不允许连接，则断开
            if (!telemetryStationConnectedEventArgs.Allowed)
                telemetryStation.OnError(new ApplicationException(telemetryStationConnectedEventArgs.Message));
        }

        public void Stop()
        {
            cts?.Cancel();
            cts = null;
            tcpListener?.Stop();
            tcpListener = null;
            TelemetryStationContext[] telemetryStations = null;
            lock (telemetryStationList)
            {
                telemetryStations = telemetryStationList.ToArray();
                telemetryStationList.Clear();
            }
            //移除事件绑定
            foreach (var telemetryStation in telemetryStations)
            {
                telemetryStation.Connected -= TelemetryStation_Connected;
                telemetryStation.Disconnected -= TelemetryStation_Disconnected;
            }
        }
    }
}
