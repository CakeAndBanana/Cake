using Cake.Json.CakeBuilders.Osu;

namespace Cake.Core.Extensions.Osu
{
    public class OsuCheckRetries
    {
        public static int Tries(int mode, int userid, int beatmapid)
        {
            int count = 0;

            var recentBuilder = new OsuUserRecentBuilder()
            {
                Mode = mode,
                Limit = "20",
                UserId = userid
            };

            var result = recentBuilder.Execute(true);

            foreach (var recent in result)
            {
                if (beatmapid == recent.beatmap_id)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }
    }
}
