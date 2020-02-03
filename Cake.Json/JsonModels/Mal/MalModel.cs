using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cake.Json.JsonModels.Mal
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
}
