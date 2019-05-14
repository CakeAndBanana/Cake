using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CakeBot.Helper.JsonStorage.Model
{
    [DataContract]
    public class ConfigModel
    {
        [DataMember]
        public string BotName { get; set; }

        [DataMember]
        public string DebugPrefix { get; set; }

        [DataMember]
        public string BotStatus { get; set; }

        [DataMember]
        public string ReleaseKey { get; set; }

        [DataMember]
        public string DebugKey { get; set; }

        [DataMember]
        public string TillerinoApi { get; set; }

        [DataMember]
        public string OsuApi { get; set; }

        [DataMember]
        public string ConnectionString { get; set; }

        [DataMember]
        public string TwitterConsumerKey { get; set; }

        [DataMember]
        public string TwitterConsumerSecret { get; set; }

        [DataMember]
        public string TwitterAccessToken { get; set; }

        [DataMember]
        public string TwitterAccessTokenSecret { get; set; }
    }
}
