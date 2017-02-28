namespace Durian.Authentication
{
    public sealed class Authorize
    {
        public string Backend { get; private set; }

        public string Uid { get; private set; }

        public string Token { get; private set; }

        public string Extra { get; private set; }

        public Authorize(string serviceName, string uid, string token, string extra)
        {
            Backend = serviceName;
            Uid = uid;
            Token = token;
            Extra = extra;
        }
    }
}
