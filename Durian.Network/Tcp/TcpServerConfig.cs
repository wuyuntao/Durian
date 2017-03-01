using System.Net;

namespace Durian.Network
{
    public sealed class TcpServerConfig
    {
        public IPEndPoint Address { get; private set; }

        public uint ReceiveBufferSize { get; private set; }

        public uint SendBufferSize { get; private set; }

        public uint BackLog { get; private set; }

        public TcpServerConfig(IPEndPoint address, uint receiveBufferSize, uint sendBufferSize, uint backLog)
        {
            Address = address;
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
            BackLog = backLog;
        }
    }
}
