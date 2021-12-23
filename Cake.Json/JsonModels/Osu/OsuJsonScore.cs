using System;

namespace Cake.Json.CakeModels.Osu
{
    public class OsuJsonScore : OsuJsonScorable
    {
        public OsuJsonBeatmap Beatmap { get; set; }
        public string score_id { get; set; }
        public string rounded_score { get; set; }
        public string username { get; set; }
        public int maxcombo { get; set; }
        public int hitted { get; set; }
        public string perfect { get; set; }
        public string enabled_mods { get; set; }
        public string user_id { get; set; }
        public string rank { get; set; }
        public double? pp { get; set; }
    }
}
