using System.Collections.Generic;

namespace Cake.Json.CakeModels.Osu
{
    public class OsuJsonMatch : OsuJsonModel
    {
        public string match_id { get; set; }
        public string name { get; set; }
        public string start_time { get; set; }
        public object end_time { get; set; }
        public List<OsuJsonGame> games { get; set; }
    }
}
