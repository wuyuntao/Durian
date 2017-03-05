using Akka.Actor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Durian.Network
{
    abstract class Connection : ReceiveActor
    {
        private readonly List<IActorRef> handlers = new List<IActorRef>();

        public Connection()
        {
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
            ReceiveAsync<Register>(async m => handlers.Add(m.Handler));
            ReceiveAsync<Unregister>(async m => handlers.Add(m.Handler));
            ReceiveAsync<Received>(async m => handlers.ForEach(h => h.Tell(m.Payload, Self)));
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
            ReceiveAsync<Send>(async m => await WriteAsync(m.Payload));
        }

        internal abstract Task WriteAsync(object payload);
    }
}
