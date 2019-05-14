using System;

namespace CakeBot.Helper.Modules.Osu.Model
{
    public class OsuJsonUserRecent : OsuJsonScorable
    {
        public string beatmap_id { get; set; }
        public int score { get; set; }
        public string rounded_score { get; set; }
        public double starrating { get; set; }
        public int maxcombo { get; set; }
        public int counttotal { get; set; }
        public int standardhit { get; set; }
        public int hitted { get; set; }
        public string perfect { get; set; }
        public string enabled_mods { get; set; }
        public string user_id { get; set; }
        public DateTimeOffset date { get; set; }
        public string rank { get; set; }
        public string accuracy { get; set; }
        public double nochokeaccuracy { get; set; }
        public double pp { get; set; }
        public double nochokepp { get; set; }
        public double completion { get; set; }
    }
}
