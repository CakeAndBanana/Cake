using System;
using System.Net;
using CakeBot.Helper.Logging;
using CakeBot.Helper.Modules.BF4.Model;
using Newtonsoft.Json;

namespace CakeBot.Helper.Modules.BF4
{
    public class Bf4Data
    {
        public static Bf4Model GetPlayerInfo(string platform, string player)
        {
            var url = $"https://api.bf4stats.com/api/playerInfo?plat={platform}&name={player}&opt=urls,stats,imagePaths&output=json";
            url = new WebClient().DownloadString(url);

            return JsonConvert.DeserializeObject<Bf4Model>(url); ;
        }
    }
}
