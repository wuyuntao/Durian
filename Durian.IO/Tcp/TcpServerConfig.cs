namespace Durian.IO
{
    public sealed class TcpServerConfig
    {
        public uint ReceiveBufferSize { get; private set; }

        public uint SendBufferSize { get; private set; }

        public uint BackLog { get; private set; }

        public TcpServerConfig(uint receiveBufferSize, uint sendBufferSize, uint backLog)
        {
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
            BackLog = backLog;
        }
    }
}
