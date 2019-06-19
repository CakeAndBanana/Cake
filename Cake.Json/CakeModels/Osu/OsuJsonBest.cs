using System;

namespace CakeBot.Helper.Modules.Osu.Model
{
    public class OsuJsonBest : OsuJsonModel
    {
        public string beatmap_id { get; set; }
        public int score { get; set; }
        public string rounded_score { get; set; }
        public int maxcombo { get; set; }
        public int count50 { get; set; }
        public int count100 { get; set; }
        public int count300 { get; set; }
        public int countmiss { get; set; }
        public int countkatu { get; set; }
        public int countgeki { get; set; }
        public int hitted { get; set; }
        public string perfect { get; set; }
        public string enabled_mods { get; set; }
        public string user_id { get; set; }
        public DateTime date { get; set; }
        public string rank { get; set; }
        public string accuracy { get; set; }
        public double pp { get; set; }
        public double calculated_accuracy { get; set; }
    }
}
