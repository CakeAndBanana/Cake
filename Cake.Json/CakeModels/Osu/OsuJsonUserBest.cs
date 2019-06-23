using System;
using System.Diagnostics.Contracts;

namespace Cake.Json.CakeModels.Osu
{
    public class OsuJsonUserBest : OsuJsonScorable
    {
        public int play_number { get; set; }
        public int beatmap_id { get; set; }
        public string score { get; set; }
        public double starrating { get; set; }
        public string maxcombo { get; set; }
        public string perfect { get; set; }
        public int enabled_mods { get; set; }
        public string user_id { get; set; }
        public DateTime date { get; set; }
        public string rank { get; set; }
        public double pp { get; set; }
        public string accuracy { get; set; }
        public int calculated_pp { get; set; }
    }
}
