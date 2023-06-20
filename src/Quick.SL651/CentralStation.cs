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
        private CentralStationOptions options;
        private TcpListener tcpListener;
        private CancellationTokenSource cts;
        public CentralStation(CentralStationOptions options)
        {
            this.options = options;
        }

        public void Start()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            tcpListener = new TcpListener(options.IPAddress, options.Port);
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
                var tsContext = new TelemetryStationContext(client, cancellationToken);
                tsContext.Start();
            });
        }

        public void Stop()
        {
            cts?.Cancel();
            cts = null;
            tcpListener.Stop();
        }
    }
}
