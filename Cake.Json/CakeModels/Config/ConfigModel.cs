using System.Runtime.Serialization;

namespace Cake.Json.JsonModels
{
    [DataContract]
    public class ConfigModel
    {
        [DataMember]
        public string BotKey { get; set; }

        [DataMember]
        public string ConnectionString { get; set; }

        [DataMember]
        public bool LogEnabled { get; set; }
    }
}
