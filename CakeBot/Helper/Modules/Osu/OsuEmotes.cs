namespace CakeBot.Helper.Modules.Osu
{
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

}
