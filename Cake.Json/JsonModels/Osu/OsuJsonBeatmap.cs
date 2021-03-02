using System;

namespace Cake.Json.CakeModels.Osu
{
    public class OsuJsonBeatmap : OsuJsonModel
    {
        public int beatmapset_id { get; set; }
        public int beatmap_id { get; set; }
        public int approved { get; set; }
        public int total_length { get; set; }
        public int hit_length { get; set; }
        public string version { get; set; }
        public string file_md5 { get; set; }
        public double diff_size { get; set; }
        public double diff_overall { get; set; }
        public double diff_approach { get; set; }
        public double diff_drain { get; set; }
        public string mode { get; set; }
        public DateTimeOffset? approved_date { get; set; }
        public DateTimeOffset last_update { get; set; }
        public string artist { get; set; }
        public string title { get; set; }
        public string complete_title { get; set; }
        public string creator { get; set; }
        public double bpm { get; set; }
        public string source { get; set; }
        public string tags { get; set; }
        public int genre_id { get; set; }
        public int language_id { get; set; }
        public int favourite_count { get; set; }
        public int playcount { get; set; }
        public int passcount { get; set; }
        public int max_combo { get; set; }
        public double difficultyrating { get; set; }
        public string thumbnail { get; set; }
        public string beatmapset_url { get; set; }
        public string beatmap_url { get; set; }
        public string download { get; set; }
        public string download_no_video { get; set; }
        public string osu_direct { get; set; }
        public string bloodcat { get; set; }
    }
}
