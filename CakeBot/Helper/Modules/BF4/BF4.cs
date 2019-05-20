using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using CakeBot.Helper.Exceptions;
using CakeBot.Modules.Services;
using Discord;
using Newtonsoft.Json;
using CakeBot.Helper.Modules.BF4.Model;

namespace CakeBot.Helper.Modules.BF4
{
    public class Bf4Stats
    {
        public static Bf4Data GetPlayerInfo(string platform, string player)
        {
            string url = $"https://api.bf4stats.com/api/playerInfo?plat={platform}&name={player}&opt=urls,stats,imagePaths&output=json";
            try
            {
                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    client.BaseAddress = new Uri(url);
                    HttpResponseMessage response = client.GetAsync("").Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<Bf4Data>(result);
                }
            }
            catch (CakeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public string RankToUrl(int Rank) => $"http://www.cakebot.org/resource/bf/r{Rank}.png";
    }
}
