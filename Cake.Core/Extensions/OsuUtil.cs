using Cake.Core.Discord.Embeds;
using Cake.Core.Exceptions;
using Cake.Json.CakeBuilders.Osu;
using System;
using System.Linq;

namespace Cake.Core.Extensions
{
    public class OsuMode
    {
        protected OsuMode()
        {
        }

        private static readonly string OsuOfficialName = "osu!";
        private static readonly string TaikoOfficialName = "osu!taiko";
        private static readonly string CtbOfficialName = "osu!catch";
        private static readonly string ManiaOfficialName = "osu!mania";

        public static readonly string[] OsuNames = { "0", OsuOfficialName, "osu", "std", "standard" };
        public static readonly string[] TaikoNames = { "1", TaikoOfficialName, "taiko" };
        public static readonly string[] CTBNames = { "2", CtbOfficialName, "ctb", "catch the beat", "catch" };
        public static readonly string[] ManiaNames = { "3", ManiaOfficialName, "mania" };

        public static string GetOfficialName(string number)
        {
            switch (number)
            {
                case "0":
                    return OsuOfficialName;
                case "1":
                    return TaikoOfficialName;
                case "2":
                    return CtbOfficialName;
                case "3":
                    return ManiaOfficialName;
            }
            return OsuOfficialName;
        }

        public static OsuModeEnum GetOsuMode(string name)
        {
            name = name.ToLowerInvariant();

            if (OsuNames.Any(osuName => osuName == name))
            {
                return OsuModeEnum.osu;
            }

            if (TaikoNames.Any(osuName => osuName == name))
            {
                return OsuModeEnum.Taiko;
            }

            if (CTBNames.Any(osuName => osuName == name))
            {
                return OsuModeEnum.Catch;
            }

            if (ManiaNames.Any(osuName => osuName == name))
            {
                return OsuModeEnum.Mania;
            }

            throw new CakeException($"No osu! mode found with the following name: `{name}`");
        }
    }

    public class OsuEmoteCodes
    {
        public static readonly string EmoteX = "<:miss:486635818224713739>";
        public static readonly string Emote50 = "<:hit50:486637577286189056>";
        public static readonly string Emote100 = "<:hit100:486637577181593610>";
        public static readonly string Emote300 = "<:hit300:486637577202302976>";
    }

    public static class OsuEmotes
    {
        public static string LevelEmotes(this string rank)
        {
            switch (rank)
            {
                case "F":
                    return "<:rankingF:487313612289998868>";
                case "D":
                    return "<:rankingD:487304280827494410>";
                case "C":
                    return "<:rankingC:487304280496406528>";
                case "B":
                    return "<:rankingB:487304280680693766>";
                case "A":
                    return "<:rankingA:487304280534155267>";
                case "S":
                    return "<:rankingS:487304280827494420>";
                case "SH":
                    return "<:rankingSH:487304280517115904>";
                case "X":
                    return "<:rankingX:487304280844533770>";
                case "XH":
                    return "<:rankingXH:487304281070764063>";
                default:
                    return "No valid rank given";
            }
        }
    }

    internal class OsuMods
    {
        public static string Modnames(int mods)
        {
            string modString;
            if (mods > 0)
            {
                modString = "+";
            }
            else
            {
                modString = "";
            }

            if (IsBitSet(mods, 0))
                modString += "NF";
            if (IsBitSet(mods, 1))
                modString += "EZ";
            if (IsBitSet(mods, 8))
                modString += "HT";
            if (IsBitSet(mods, 3))
                modString += "HD";
            if (IsBitSet(mods, 4))
                modString += "HR";
            if (IsBitSet(mods, 6) && !IsBitSet(mods, 9))
                modString += "DT";
            if (IsBitSet(mods, 9))
                modString += "NC";
            if (IsBitSet(mods, 10))
                modString += "FL";
            if (IsBitSet(mods, 5))
                modString += "SD";
            if (IsBitSet(mods, 14))
                modString += "PF";
            if (IsBitSet(mods, 7))
                modString += "RX";
            if (IsBitSet(mods, 11))
                modString += "AT";
            if (IsBitSet(mods, 12))
                modString += "SO";
            return modString;
        }

        private static bool IsBitSet(int mods, int pos) =>
            (mods & (1 << pos)) != 0;
    }

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
