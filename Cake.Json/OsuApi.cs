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
        //Osu urls
        public const string OsuApiBaseUrl = "https://osu.ppy.sh/api/";
        public const string OsuDownload = "https://osu.ppy.sh/d/";
        public const string OsuUserUrl = "https://osu.ppy.sh/u/";
        public const string OsuAvatarUrl = "https://a.ppy.sh/";
        public const string OsuFlagUrl = "https://osu.ppy.sh/images/flags/";
        public const string OsuOldFlagUrl = "https://s.ppy.sh/images/flags/";

        //Osu game links
        public const string OsuDirect = "osu://s/";
        public const string OsuSpectate = "osu://spectate/";

        //Thirdparty urls
        public const string Bloodcat = "https://bloodcat.com/osu/s/";
        public const string OsuTrack = "https://ameobea.me/osutrack/user/";
        public const string OsuStats = "https://osustats.ppy.sh/u/";
        public const string OsuSkills = "http://osuskills.tk/user/";
        public const string OsuChan = "https://syrin.me/osuchan/u/";

        public const string EmoteX = "<:miss:486635818224713739>";
        public const string Emote50 = "<:hit50:486637577286189056>";
        public const string Emote100 = "<:hit100:486637577181593610>";
        public const string Emote300 = "<:hit300:486637577202302976>";



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
