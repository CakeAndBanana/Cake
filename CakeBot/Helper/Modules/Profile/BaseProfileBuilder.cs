using System.IO;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Text;
using SixLabors.Primitives;

namespace CakeBot.Helper.Modules.Profile
{
    public class BaseProfileBuilder
    {
        private static readonly Point AvatarLocation = new Point(215, 119);
        private static readonly Point AvatarBorderLocation = new Point(214, 118);
        private static readonly Point NameBoxLocation = new Point(10, 134);
        private static readonly Point NameLocation = new Point(14, 147);
        public static readonly Point XpBarBorderLocation = new Point(214, 195);
        private static readonly Point XpBarBackgroundLocation = new Point(214, 195);
        private static readonly Point XpBarLocation = new Point(215, 196);
        private static readonly Point InfoBoxLocation = new Point(10, 211);
        private static readonly Point InfoBoxBorderLocation = new Point(9, 210);
        private static readonly Point LevelTextLocation = new Point(225, 220);
        private static readonly Point TextBalanceLocation = new Point(14, 219);
        private static readonly Point TextTotalxpLocation = new Point(14, 245);
        private static readonly Point TextCastsLocation = new Point(15, 270);
        private static readonly Point BalanceLocation = new Point(85, TextBalanceLocation.Y);
        private static readonly Point TotalxpLocation = new Point(85, TextTotalxpLocation.Y);
        private static readonly Point CastsLocation = new Point(85, TextCastsLocation.Y);
        private static readonly Point GlobeLocation = new Point(162, 215);
        private static string baseAddress = Directory.GetCurrentDirectory();
        private static readonly string BalanceText = "Balance";
        private static readonly string TotalxpText = "Total XP";
        private static readonly string TotalCasts = "Casts";

        public string Background { get; set; }
        public float BoxOpacity { get; set; }
        public Image<Rgba32> NameBox { get; set; }
        public Image<Rgba32> InfoBox { get; set; }
        public Image<Rgba32> InfoBoxBorder { get; set; }
        public Image<Rgba32> Avatar { get; set; }
        public Image<Rgba32> AvatarBorder { get; set; }
        public Image<Rgba32> XpBackground { get; set; }
        public Image<Rgba32> Globe { get; set; }
        public Image<Rgba32> XpBar { get; set; }
        public Rgba32 XpBorderColor { get; set; }
        public ProfileXp ProfileXp { get; set; }
        public Font XpFont { get; set; }
        public string Username { get; set; }
        public Font UsernameFont { get; set; }
        public string LevelText { get; set; }
        public Font MainFont { get; set; }
        public Font LevelFont { get; set; }
        public Font InfoFont { get; set; }
        public Font RankFont { get; set; }
        public Rectangle XpBorder { get; set; }
        public UserStats UserStats { get; set; }
        
        public void Build(string saveLocation)
        {
            using (var image = Image.Load( baseAddress+@"\Images\" + Background + ".png"))
            {
                var xpString = ProfileXp.CurrentXp + " / " + ProfileXp.NextLevelXp;
                var xpStringLoc = new Point(XpBorder.X + (XpBorder.Width / 3) - 3, XpBorder.Y + 2);
                image.Mutate(x => x
                        .DrawImage(NameBox, BoxOpacity, NameBoxLocation) // Name Box
                        .DrawImage(InfoBox, BoxOpacity, InfoBoxLocation) // Info Box
                        .DrawImage(InfoBoxBorder, 1, InfoBoxBorderLocation) // Info Box Border
                        .DrawImage(Avatar, 1, AvatarLocation) // Avatar
                        .DrawImage(AvatarBorder, 1, AvatarBorderLocation) // Avatar Border
                        .DrawImage(XpBackground, 1, XpBarBackgroundLocation) // Background inside xp bar
                        .Draw(XpBorderColor, 1, XpBorder) // XP Bar Border
                        .DrawImage(XpBar, 1, XpBarLocation) // XP Bar (Percentage to next level) itself
                        .DrawText(xpString, XpFont, Rgba32.Black, xpStringLoc) // XP Values inside the xp bar
                        .DrawText(Username, UsernameFont, UserStats.ColorHex, NameLocation) // Username
                        .DrawText(LevelText, MainFont, UserStats.ColorHex, LevelTextLocation) // "Level"
                        .DrawText(ProfileXp.Level.ToString(), LevelFont, UserStats.ColorHex, ProfileXp.GetLevelLocation()) // Current Level of the player
                        .DrawText(BalanceText,InfoFont ,UserStats.ColorHex, TextBalanceLocation)
                        .DrawText(TotalxpText, InfoFont, UserStats.ColorHex, TextTotalxpLocation)
                        .DrawText(TotalCasts, InfoFont, UserStats.ColorHex, TextCastsLocation)
                        .DrawText(UserStats.Balance, InfoFont, UserStats.ColorHex, BalanceLocation)
                        .DrawText(UserStats.TotalXp, InfoFont, UserStats.ColorHex, TotalxpLocation)
                        .DrawText(UserStats.Casts, InfoFont, UserStats.ColorHex, CastsLocation)
                        .DrawImage(Globe, 1, GlobeLocation)
                        .DrawText(UserStats.RankString,RankFont, UserStats.ColorHex, UserStats.GetRankLocation())
                );

                image.Save(saveLocation);
            }
        }
    }
}
