using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Modules.MAL.Model;
using Newtonsoft.Json;
using System;
using System.Net;

namespace CakeBot.Helper.Modules.MAL
{
    public class MalData
    {
        public static MALModel GetDesObject(string url)
        {
            return JsonConvert.DeserializeObject<MALModel>(url);
        }

        private const string BaseUrl = "https://api.jikan.moe/v3";

        public static MALModel GetRandomAnimeByCat(int genre_id)
        {
            var url = $"{BaseUrl}/genre/anime/{genre_id}";
            try
            {
                url = new WebClient().DownloadString(url);
                url = MalHelper.RandomPage(genre_id, GetDesObject(url).item_count, "anime");
            }
            catch(Exception e)
            {
                throw new CakeException(e.Message);
            }

            return JsonConvert.DeserializeObject<MALModel>(url);
        }
        public static MALModel GetRandomMangaByCat(int genre_id)
        {
            var url = $"{BaseUrl}/genre/manga/{genre_id}";

            try
            {
                url = new WebClient().DownloadString(url);
                url = MalHelper.RandomPage(genre_id, GetDesObject(url).item_count, "manga");
            }
            catch
            {
                return null;
            }

            return JsonConvert.DeserializeObject<MALModel>(url);
        }
        public static MALModel SearchAnime(string name)
        {
            var url = $"{BaseUrl}/search/anime?q={name}&page=1";

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
            var url = $"{BaseUrl}/search/manga?q={name}&page=1";

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
}
