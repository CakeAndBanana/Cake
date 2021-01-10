using System.Runtime.Serialization;

namespace Cake.Json.JsonModels
{
    [DataContract]
    public class ConfigModel
    {
        [DataMember]
        public string Version { get; set; }

        //Api keys
        [DataMember]
        public string BotKey { get; set; }
        [DataMember]
        public string OsuApiKey { get; set; }

        //Database Settings
        [DataMember]
        public string ConnectionString { get; set; }
        [DataMember]
        public string DatabaseProvider { get; set; }

        //Bot Settings
        [DataMember]
        public bool LogEnabled { get; set; }
    }
}
