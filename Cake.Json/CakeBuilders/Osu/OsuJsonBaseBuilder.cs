using System.Net;
using System.Text;
using Cake.Json.CakeModels.Osu;
using Newtonsoft.Json;

namespace Cake.Json.CakeBuilders.Osu
{
    public abstract class OsuJsonBaseBuilder<T> where T : OsuJsonModel
    {
        private readonly string _apiKey = CakeJson.GetConfig().OsuApiKey;

        public string Build(OsuApiRequest request)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.Append(OsuUtil.OsuApiBaseUrl);

            switch (request)
            {
                case OsuApiRequest.Beatmap:
                    urlBuilder.Append("get_beatmaps");
                    break;
                case OsuApiRequest.User:
                    urlBuilder.Append("get_user");
                    break;
                case OsuApiRequest.Scores:
                    urlBuilder.Append("get_scores");
                    break;
                case OsuApiRequest.BestPerformance:
                    urlBuilder.Append("get_user_best");
                    break;
                case OsuApiRequest.RecentPlayed:
                    urlBuilder.Append("get_user_recent");
                    break;
                case OsuApiRequest.Multiplayer:
                    urlBuilder.Append("get_match");
                    break;
                default:
                    break;
            }

            urlBuilder.Append("?");
            urlBuilder.Append("k=");
            urlBuilder.Append(_apiKey);
            return Build(urlBuilder);
        }

        protected T[] ExecuteJson(OsuApiRequest request)
        {
            var html = Build(request);
            html = new WebClient().DownloadString(html);
            return JsonConvert.DeserializeObject<T[]>(html);
        }

        public abstract string Build(StringBuilder urlBuilder);
    }
}
