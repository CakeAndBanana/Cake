using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using CakeBot.Helper.Exceptions;
using Discord;
using Newtonsoft.Json.Linq;

namespace CakeBot.Helper.Modules.BF4
{
    // fdvvc dfb
    public class Bf4Stats
    {
        public string Player { get; set; }
        public string Platform { get; set; }
        public int Level { get; set; }
        public int Skill { get; set; }
        public double Kills { get; set; }
        public double Deaths { get; set; }
        public double KDR { get; set; }
        public int Killstreak { get; set; }
        public int Headshots { get; set; }
        public int LongestHeadshot { get; set; }
        public double Wins { get; set; }
        public double Losses { get; set; }
        public string Winrate { get; set; }
        public double Accuracy { get; set; }
        public string TimePlayed { get; set; }
        public int TotalScore { get; set; }
        public string ____________________ { get; set; }


        public async Task<Embed> GetStats(string platform, string player, IUser user)
        {
            var builder = new CakeEmbedBuilder(EmbedType.Success)
            {
                Author = new EmbedAuthorBuilder() {IconUrl = user.GetAvatarUrl(), Name = user.Username,}
            };

            #region BS
            string thumbUrl = null;
            string url = $"https://api.bf4stats.com/api/playerInfo?plat={platform}&name={player}&opt=urls,stats,imagePaths&output=json";
            try
            {
                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    client.BaseAddress = new Uri(url);
                    HttpResponseMessage response = client.GetAsync("").Result;
                    response.EnsureSuccessStatusCode();
                    string result = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(result);
                    Player = json["player"]["name"].ToString();
                    Kills = Convert.ToInt32(json["stats"]["kills"]);
                    Deaths = Convert.ToInt32(json["stats"]["deaths"]);
                    double kdr = (Kills / Deaths);
                    KDR = Math.Round(kdr, 2);
                    Platform = json["player"]["plat"].ToString();
                    LongestHeadshot = Convert.ToInt32(json["stats"]["longestHeadshot"]);
                    int seconds = Convert.ToInt32(json["player"]["timePlayed"]);
                    TimePlayed = $"{TimeSpan.FromSeconds(seconds).Days} day(s) {TimeSpan.FromSeconds(seconds).Hours}hrs {TimeSpan.FromSeconds(seconds).Minutes}min";
                    ____________________ = json["player"]["blPlayer"].ToString();
                    Killstreak = Convert.ToInt32(json["stats"]["killStreakBonus"]);
                    TotalScore = Convert.ToInt32(json["player"]["score"]);
                    double shots = Convert.ToDouble(json["stats"]["shotsFired"]);
                    double hits = Convert.ToDouble(json["stats"]["shotsHit"]);
                    Accuracy = Math.Round((hits / shots * 100), 2);
                    Headshots = Convert.ToInt32(json["stats"]["headshots"]);
                    Skill = Convert.ToInt32(json["stats"]["skill"]);
                    double wins = Convert.ToDouble(json["stats"]["numWins"]);
                    double losses = Convert.ToDouble(json["stats"]["numLosses"]);
                    double rounds = wins + losses;
                    Wins = wins;
                    Losses = losses;
                    Winrate = Math.Round((wins / rounds * 100), 1) + "%";
                    thumbUrl = json["player"]["rank"]["imgLarge"].ToString();
                    Level = Convert.ToInt32(json["player"]["rank"]["nr"]);
                }
            }
            catch (CakeException ex)
            {
                Console.WriteLine(ex.Message);
                return builder.Build();
            }

            builder.WithFooter("Thanks for using!");
            builder.ThumbnailUrl = "http://www.xgamerz.org/static/" + thumbUrl;

            #endregion

            var properties = GetType().GetProperties();
            foreach (var p in properties)
            {
                var myVal = p.GetValue(this);
                builder.AddField(x =>
                {
                    x.Name = $"**{p.Name}**";
                    x.Value = $"{ p.GetValue(this)}";
                    x.IsInline = true;
                });
            }

            return builder.Build();
        }
    }
}
