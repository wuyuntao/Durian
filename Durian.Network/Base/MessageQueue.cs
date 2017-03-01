using System;
using System.Collections.Generic;

namespace Durian.Network
{
    sealed class MessageQueue
    {
        private Queue<byte[]> queue = new Queue<byte[]>();
        private int passedBytes;
        private int pendingMessageSize;

        public void Enqueue(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.queue.Enqueue(data);
        }

        public void Enqueue(byte[] data, int offset, int count)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (offset < 0 || offset >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (count <= 0 || offset + count > data.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            var bytes = new byte[count];
            Array.Copy(data, offset, bytes, 0, count);

            this.queue.Enqueue(data);
        }

        public byte[] Dequeue()
        {
            if (pendingMessageSize == 0)
            {
                var size = DequeueSize();
                if (size == null)
                    return null; // There're not enough bytes which can express a complete varint

                pendingMessageSize = size.Value;
            }

            if (pendingMessageSize > 0)
            {
                var data = DequeueBytes(pendingMessageSize);
                if (data != null)
                {
                    pendingMessageSize = 0;

                    return data;
                }
            }

            return null;
        }

        public IEnumerable<byte[]> DequeueAll()
        {
            for (var bytes = Dequeue(); bytes != null; bytes = Dequeue())
                yield return bytes;
        }

        private int? DequeueSize()
        {
            var bytes = DequeueBytes(sizeof(int));

            if (bytes != null)
                return BitConverter.ToInt32(bytes, 0);
            else
                return null;
        }

        private byte[] DequeueBytes(int bytes)
        {
            if (!HasBytes(bytes))
                return null;

            var dest = new byte[bytes];
            int destIndex = 0;

            while (destIndex < bytes)
            {
                var src = this.queue.Peek();
                var copyBytes = Math.Min(src.Length - this.passedBytes, bytes - destIndex);

                Array.Copy(src, this.passedBytes, dest, destIndex, copyBytes);

                destIndex += copyBytes;
                this.passedBytes += copyBytes;

                if (this.passedBytes == src.Length)
                {
                    this.queue.Dequeue();
                    this.passedBytes = 0;
                }
            }

            return dest;
        }

        private bool HasBytes(int bytes)
        {
            if (bytes == 0)
                throw new ArgumentException("bytes must > 0");

            if (this.queue.Count == 0)
                return false;

            bytes += this.passedBytes;

            foreach (var data in this.queue)
            {
                bytes -= data.Length;

                if (bytes <= 0)
                    return true;
            }

            return false;
        }
    }
}