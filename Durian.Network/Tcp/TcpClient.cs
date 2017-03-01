using System;
using System.Net.Sockets;
using NetClient = System.Net.Sockets.TcpClient;

namespace Durian.Network
{
    public class TcpClient : Client
    {
        private readonly TcpClientConfig config;
        private TcpConnection connection;

        public TcpClient(TcpClientConfig config)
        {
            this.config = config;

            var client = new NetClient();
            client.BeginConnect(config.Address.Address, config.Address.Port, OnConnect, client);
        }

        protected override void DisposeManaged()
        {
            lock (this)
            {
                if (connection != null)
                    connection.Dispose();
            }

            base.DisposeManaged();
        }

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                var client = (NetClient)ar.AsyncState;
                client.EndConnect(ar);

                Connected(new TcpConnection(client));
            }
            catch (SocketException ex)
            {
                ConnectError(ex);
            }
        }
    }
}
