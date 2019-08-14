using System;
using System.Collections.Generic;

namespace Cake.Json.CakeModels.Osu
{

    public class OsuJsonUser : OsuJsonModel
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public string count300 { get; set; }
        public string count100 { get; set; }
        public string count50 { get; set; }
        public string playcount { get; set; }
        public string ranked_score { get; set; }
        public string total_score { get; set; }
        public string pp_rank { get; set; }
        public string level { get; set; }
        public double pp_raw { get; set; }
        public string accuracy { get; set; }
        public string count_rank_ss { get; set; }
        public string count_rank_s { get; set; }
        public string count_rank_a { get; set; }
        public string country { get; set; }
        public string pp_country_rank { get; set; }
        public DateTime join_date { get; set; }
        public List<OsuJsonEvent> events { get; set; }
        public string url { get; set; }
        public string image { get; set; }
        public string flag { get; set; }
        public string flag_old { get; set; }
        public string osutrack { get; set; }
        public string osustats { get; set; }
        public string osuskills { get; set; }
        public string osuchan { get; set; }
        public string spectateUser { get; set; }
    }
}
