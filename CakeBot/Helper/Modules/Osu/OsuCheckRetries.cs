using CakeBot.Helper.Modules.Osu.Builder;

namespace CakeBot.Helper.Modules.Osu
{
    public class OsuCheckRetries
    {
        public static int Tries(string mode, string userid, int beatmapid)
        {
            int count = 0;

            var recentBuilder = new OsuUserRecentBuilder()
            {
                Mode = mode,
                Limit = "50",
                UserId = userid
            };

            var result = recentBuilder.Execute(false, true);

            foreach(var recent in result)
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
