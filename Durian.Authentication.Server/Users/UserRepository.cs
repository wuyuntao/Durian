using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Event;

namespace Durian.Authentication
{
    public class UserRepository : ReceiveActor
    {
        private readonly ILoggingAdapter log = Logging.GetLogger(Context);

        private readonly IActorRef region;

        public UserRepository()
        {
            var authenticationConfig = AuthenticationConfigFactory.Default();
            var settings = Context.System.Settings;
            settings.InjectTopLevelFallback(authenticationConfig);

            region = ClusterSharding.Get(Context.System).Start(
                "user",
                Props.Create<User>(),
                ClusterShardingSettings.Create(Context.System),
                new ShardMessageExtractor());

            Receive<ShardEnvelope>(m => region.Tell(m, Sender));
        }
    }
}
