namespace Durian.Network
{
    public abstract class Server : Disposable
    {
        public event ConnectedHandler OnConnected;

        protected void Connected(Connection connection)
        {
            if (OnConnected != null)
                OnConnected(connection);
        }
    }
}
