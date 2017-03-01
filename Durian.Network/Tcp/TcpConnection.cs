using System;
using System.Net.Sockets;
using NetClient = System.Net.Sockets.TcpClient;

namespace Durian.Network
{
    public class TcpConnection : Connection
    {
        private readonly NetClient client;

        private readonly NetworkStream stream;
        private readonly byte[] receiveBuffer;
        private readonly MessageQueue messageQueue = new MessageQueue();

        public TcpConnection(NetClient client)
        {
            this.client = client;

            receiveBuffer = new byte[client.ReceiveBufferSize];
            stream = this.client.GetStream();
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, OnRead, null);
        }

        protected override void DisposeManaged()
        {
            client.Close();

            base.DisposeManaged();
        }

        private void OnRead(IAsyncResult ar)
        {
            try
            {
                var size = stream.EndRead(ar);
                if (size > 0)
                {
                    messageQueue.Enqueue(receiveBuffer, 0, size);

                    foreach (var message in messageQueue.DequeueAll())
                        MessageReceived(message);
                }
            }
            catch (SocketException ex)
            {
                ReceiveError(ex);
            }
            finally
            {
                if (!client.Connected)
                    stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, OnRead, null);
                else
                    Disconnected();
            }
        }

        public override void Send(byte[] payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            var payloadSize = BitConverter.GetBytes(payload.Length);
            var buffer = new byte[payloadSize.Length + payload.Length];
            Array.Copy(payloadSize, 0, buffer, 0, payloadSize.Length);
            Array.Copy(payload, 0, buffer, payloadSize.Length, payload.Length);

            stream.BeginWrite(buffer, 0, buffer.Length, OnWrite, null);
        }

        private void OnWrite(IAsyncResult ar)
        {
            try
            {
                stream.EndWrite(ar);
            }
            catch (SocketException ex)
            {
                SendError(ex);
            }
        }
    }
}
