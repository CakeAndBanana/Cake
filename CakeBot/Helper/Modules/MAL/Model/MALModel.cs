using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CakeBot.Helper.Modules.MAL.Model
{
    [DataContract]
    public class MalModel
    {
        [DataMember]
        public int item_count { get; set; }
        [DataMember]
        public List<MalData> anime { get; set; }
        [DataMember]
        public List<MalData> manga { get; set; }
        [DataMember]
        public List<MalData> results { get; set; }
    }

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
        public Genres[] genres { get; set; } = null;
        [DataMember]
        public decimal? score { get; set; }
        [DataMember]
        public Boolean r18 { get; set; }
        [DataMember]
        public int? members { get; set; }
        [DataMember]
        public string source { get; set; }
        [DataMember]
        public int? volumes { get; set; }
    }
    public class Genres
    {
        [DataMember]
        public string name { get; set; }
    }
}
