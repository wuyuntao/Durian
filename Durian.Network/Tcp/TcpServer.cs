using System;
using System.Net.Sockets;

namespace Durian.Network
{
    public class TcpServer : Server
    {
        private readonly TcpServerConfig config;
        private readonly TcpListener listener;

        public TcpServer(TcpServerConfig config)
        {
            this.config = config;

            listener = new TcpListener(config.Address);

            listener.Server.ReceiveBufferSize = (int)config.ReceiveBufferSize;
            listener.Server.SendBufferSize = (int)config.SendBufferSize;

            listener.Start((int)config.BackLog);
            listener.BeginAcceptTcpClient(OnAccept, null);
        }

        protected override void DisposeManaged()
        {
            listener.Stop();

            base.DisposeManaged();
        }

        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                var client = listener.EndAcceptTcpClient(ar);
                var connection = new TcpConnection(client);

                Connected(connection);
            }
            finally
            {
                listener.BeginAcceptTcpClient(OnAccept, null);
            }
        }
    }
}
