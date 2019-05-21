using System.Linq;
using CakeBot.Helper.Exceptions;

namespace CakeBot.Helper.Modules.Osu
{
    public enum OsuModeEnum
    {
        osu,
        Taiko,
        Catch,
        Mania
    }

    public class OsuMode
    {
        private static string OsuOfficialName = "osu!";
        private static string TaikoOfficialName = "osu!taiko";
        private static string CtbOfficialName = "osu!catch";
        private static string ManiaOfficialName = "osu!mania";

        public static string[] OsuNames = { "0", OsuOfficialName, "osu", "std", "standard" };
        public static string[] TaikoNames = { "1", TaikoOfficialName, "taiko" };
        public static string[] CTBNames = { "2", CtbOfficialName, "ctb", "catch the beat", "catch" };
        public static string[] ManiaNames = { "3", ManiaOfficialName, "mania" };

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
                default:
                    return OsuOfficialName;
            }
        }

        public static OsuModeEnum GetOsuMode(string name)
        {
            name = name.ToLower();

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
            
            throw new CakeException($"No osu! mode found with the following name: {name}");
        }
    }
}
