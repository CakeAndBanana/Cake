using Cake.Json.CakeModels.Osu;

namespace Cake.Json
{
    public enum OsuApiRequest
    {
        Beatmap,
        User,
        Scores,
        BestPerformance,
        RecentPlayed,
        Multiplayer
    }

    public class OsuUtil
    {
        protected OsuUtil()
        {
        }

        //Osu urls
        public static readonly string OsuApiBaseUrl = "https://osu.ppy.sh/api/";
        public static readonly string OsuDownload = "https://osu.ppy.sh/d/";
        public static readonly string OsuUserUrl = "https://osu.ppy.sh/u/";
        public static readonly string OsuAvatarUrl = "https://a.ppy.sh/";
        public static readonly string OsuFlagUrl = "https://osu.ppy.sh/images/flags/";
        public static readonly string OsuOldFlagUrl = "https://s.ppy.sh/images/flags/";

        //Osu game links
        public static readonly string OsuDirect = "osu://s/";
        public static readonly string OsuSpectate = "osu://spectate/";

        //Thirdparty urls
        public static readonly string Bloodcat = "https://bloodcat.com/osu/s/";
        public static readonly string OsuTrack = "https://ameobea.me/osutrack/user/";
        public static readonly string OsuStats = "https://osustats.ppy.sh/u/";
        public static readonly string OsuSkills = "http://osuskills.tk/user/";
        public static readonly string OsuChan = "https://syrin.me/osuchan/u/";

        public static string GetOsuUserPictureUrl(int userId)
        {
            return OsuAvatarUrl + userId + "_1512645682.png";
        }

        public static void GetCalculatedAccuracy(OsuJsonScorable item, string mode)
        {
            switch (mode)
            {
                case "0":
                    item.calculated_accuracy = CalculateOsuAccuracy(item);
                    break;
                case "1":
                    item.calculated_accuracy = CalculateTaikoAccuracy(item);
                    break;
                case "2":
                    item.calculated_accuracy = CalculateCatchAccuracy(item);
                    break;
                case "3":
                    item.calculated_accuracy = CalculateManiaAccuracy(item);
                    break;
            }
        }

        public static double CalculateOsuAccuracy(OsuJsonScorable item)
        {
            return 100.0 * (6 * item.count300 + 2 * item.count100 + item.count50) /
                   (6 * (item.count50 + item.count100 + item.count300 + item.countmiss));
        }

        public static double CalculateTaikoAccuracy(OsuJsonScorable item)
        {
            return 100.0 * (2 * item.count300 + item.count100) /
                   (2 * (item.count300 + item.count100 + item.countmiss));
        }

        public static double CalculateCatchAccuracy(OsuJsonScorable item)
        {
            return 100.0 * (item.count300 + item.count100 + item.count50) /
                   (item.count300 + item.count100 + item.count50 + item.countkatu +
                    item.countmiss);
        }

        public static double CalculateManiaAccuracy(OsuJsonScorable item)
        {
            return 100.0 * (6 * item.countgeki + 6 * item.count300 + 4 * item.countkatu + 2 * item.count100 +
                            item.count50) /
                   (6 * (item.count50 + item.count100 + item.count300 + item.countmiss + item.countgeki +
                         item.countkatu));
        }
    }
}
