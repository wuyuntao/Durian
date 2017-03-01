using System.Net;

namespace Durian.Network
{
    public sealed class TcpClientConfig
    {
        public IPEndPoint Address { get; private set; }

        public uint ReceiveBufferSize { get; private set; }

        public uint SendBufferSize { get; private set; }

        public TcpClientConfig(IPEndPoint address, uint receiveBufferSize, uint sendBufferSize)
        {
            Address = address;
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
        }
    }
}
