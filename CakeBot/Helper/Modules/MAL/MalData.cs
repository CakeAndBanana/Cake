using CakeBot.Helper.Modules.MAL.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CakeBot.Helper.Modules.MAL
{
    public class MalData
    {
        static string baseUrl = "https://api.jikan.moe/v3";
        public static MALModel GetRandomAnimeByCat(int genre_id)
        {
            var url = $"{baseUrl}/genre/anime/{genre_id}";

            try
            {
                url = new WebClient().DownloadString(url);
            }
            catch
            {
                return null;
            }

            return JsonConvert.DeserializeObject<MALModel>(url);
        }
        public static MALModel GetRandomMangaByCat(int genre_id)
        {
            var url = $"{baseUrl}/genre/manga/{genre_id}";

            try
            {
                url = new WebClient().DownloadString(url);
            }
            catch
            {
                return null;
            }

            return JsonConvert.DeserializeObject<MALModel>(url);
        }
        public static MALModel SearchAnime(string name)
        {
            var url = $"{baseUrl}/search/anime?q={name}&page=1";

            try
            {
                url = new WebClient().DownloadString(url);
            }
            catch
            {
                return null;
            }

            return JsonConvert.DeserializeObject<MALModel>(url);
        }
        public static MALModel SearchManga(string name)
        {
            var url = $"{baseUrl}/search/manga?q={name}&page=1";

            try
            {
                url = new WebClient().DownloadString(url);
            }
            catch
            {
                return null;
            }

            return JsonConvert.DeserializeObject<MALModel>(url);
        }
    }

    public enum Genres
    {
        Action = 1,
        Adventure = 2,
        Cars = 3,
        Comedy = 4,
        Dementia = 5,
        Demons = 6,
        Mystery = 7,
        Drama = 8,
        Ecchi = 9,
        Fantasy = 10,
        Game = 11,
        Hentai = 12,
        Historical = 13,
        Horror = 14,
        Kids = 15,
        Magic = 16,
        Martial_Arts = 17,
        Mecha = 18,
        Music = 19,
        Parody = 20,
        Samurai = 21,
        Romance = 22,
        School = 23,
        Sci_Fi = 24,
        Shoujo = 25,
        Shoujo_Ai = 26,
        Shounen = 27,
        Shounen_Ai = 28,
        Space = 29,
        Sports = 30,
        Super_Power = 31,
        Vampire = 32,
        Yaoi = 33,
        Yuri = 34,
        Harem = 35,
        Slice_Of_Life = 36,
        Supernatural = 37,
        Military = 38,
        Police = 39,
        Psychological = 40
    }
}
