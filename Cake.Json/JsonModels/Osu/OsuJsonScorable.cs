namespace Cake.Json.CakeModels.Osu
{
    public class OsuJsonScorable : OsuJsonModel
    {
        public int count50 { get; set; }
        public int count100 { get; set; }
        public int count300 { get; set; }
        public int countmiss { get; set; }
        public int countkatu { get; set; }
        public int countgeki { get; set; }
        public double calculated_accuracy { get; set; }
        public bool choked { get; set; } = false;
    }
}
