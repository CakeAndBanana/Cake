namespace Cake.Json.CakeModels.Osu
{
    public class OsuJsonEvent : OsuJsonModel
    {
        public string display_html { get; set; }
        public string beatmap_id { get; set; }
        public string beatmapset_id { get; set; }
        public string date { get; set; }
        public string epicfactor { get; set; }
    }
}
