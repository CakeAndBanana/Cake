using System.Runtime.Serialization;

namespace Cake.Json.JsonModels.Mal
{
    public class MalData
    {
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string image_url { get; set; }
        [DataMember]
        public string synopsis { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string airing_start { get; set; }
        [DataMember]
        public string start_date { get; set; }
        [DataMember]
        public string end_date { get; set; }
        [DataMember]
        public bool airing { get; set; }
        [DataMember]
        public int? episodes { get; set; }
        [DataMember]
        public MalGenres[] genres { get; set; } = null;
        [DataMember]
        public decimal? score { get; set; }
        [DataMember]
        public bool r18 { get; set; }
        [DataMember]
        public int? members { get; set; }
        [DataMember]
        public string source { get; set; }
        [DataMember]
        public int? volumes { get; set; }
    }
}
