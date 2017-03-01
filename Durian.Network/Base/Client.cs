using System.Net.Sockets;

namespace Durian.Network
{
    public abstract class Client : Disposable
    {
        public event SocketErrorHandler OnConnectError;

        public event SocketErrorHandler OnReceiveError;

        public event SocketErrorHandler OnSendError;

        public event MessageReceivedHandler OnMessageReceived;

        public event DisconnectedHandler OnDisconnected;

        public event ConnectedHandler OnConnected;

        private Connection connection;
        private object connectionLock = new object();

        protected override void DisposeManaged()
        {
            lock (connectionLock)
            {
                if (connection != null)
                    connection.Dispose();
            }

            base.DisposeManaged();
        }

        protected void Connected(Connection connection)
        {
            lock (connectionLock)
            {
                this.connection = connection;

                this.connection.OnMessageReceived += MessageReceived;
                this.connection.OnDisconnected += Disconnected;
                this.connection.OnReceiveError += ReceiveError;
                this.connection.OnSendError += SendError;
            }
        }

        protected void ConnectError(SocketException error)
        {
            OnConnectError?.Invoke(error);
        }

        protected void MessageReceived(byte[] payload)
        {
            OnMessageReceived?.Invoke(payload);
        }

        protected void Disconnected()
        {
            OnDisconnected?.Invoke();
        }

        protected void ReceiveError(SocketException error)
        {
            OnReceiveError?.Invoke(error);
        }

        protected void SendError(SocketException error)
        {
            OnReceiveError?.Invoke(error);
        }

        public void Send(byte[] payload)
        {
            lock (connectionLock)
            {
                if (connection != null)
                    connection.Send(payload);
            }
        }
    }
}
