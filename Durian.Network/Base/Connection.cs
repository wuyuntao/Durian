using System.Net.Sockets;

namespace Durian.Network
{
    public delegate void MessageReceivedHandler(byte[] payload);

    public delegate void DisconnectedHandler();

    public delegate void ConnectedHandler(Connection connection);

    public delegate void SocketErrorHandler(SocketException error);

    public abstract class Connection : Disposable
    {
        public event MessageReceivedHandler OnMessageReceived;

        public event DisconnectedHandler OnDisconnected;

        public event SocketErrorHandler OnReceiveError;

        public event SocketErrorHandler OnSendError;

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

        public abstract void Send(byte[] payload);
    }
}
