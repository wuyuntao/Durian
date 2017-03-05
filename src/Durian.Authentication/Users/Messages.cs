using Akka.Cluster.Sharding;
using System;

namespace Durian.Authentication
{
    class ShardEnvelope
    {
        public string EntityId { get; private set; }

        public object Message { get; private set; }

        public ShardEnvelope(string userId, object message)
        {
            EntityId = userId;
            Message = message;
        }
    }

    class ShardMessageExtractor : IMessageExtractor
    {
        public string ShardId(object message)
        {
            return (message as ShardEnvelope)?.EntityId;
        }

        public string EntityId(object message)
        {
            return (message as ShardEnvelope)?.EntityId;
        }

        public object EntityMessage(object message)
        {
            return (message as ShardEnvelope)?.Message;
        }
    }

    class CreateSession
    {
    }

    class SessionCreated
    {
        public string SessionId { get; private set; }

        public DateTime SessionExpirationTime { get; private set; }

        public SessionCreated(string sessionId, DateTime sessionExpirationTime)
        {
            SessionId = sessionId;
            SessionExpirationTime = sessionExpirationTime;
        }
    }
}
