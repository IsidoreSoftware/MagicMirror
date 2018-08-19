namespace Isidore.MagicMirror.DAL.MongoDB.Configuration
{
    public class MongoDbConfig
    {
        public string ServerUrl { get; set; }
        public bool UseSsl { get; set; }
        public int? Port { get; set; }
        public string DbName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
