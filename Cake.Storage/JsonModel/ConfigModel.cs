using System.Runtime.Serialization;

namespace Cake.Storage.JsonModel
{
    [DataContract]
    public class ConfigModel
    {
        [DataMember]
        public string BotKey { get; set; }
    }
}
