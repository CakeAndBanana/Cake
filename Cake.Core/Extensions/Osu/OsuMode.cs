using Cake.Core.Discord.Embeds;
using Cake.Core.Exceptions;
using System.Linq;

namespace Cake.Core.Extensions.Osu
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

        public static string GetOfficialName(int number)
        {
            switch (number)
            {
                case 0:
                    return OsuOfficialName;
                case 1:
                    return TaikoOfficialName;
                case 2:
                    return CtbOfficialName;
                case 3:
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
}
