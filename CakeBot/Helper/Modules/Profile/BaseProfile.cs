using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Queries;
using Discord;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;
using Image = SixLabors.ImageSharp.Image;
using UserStatus = Discord.UserStatus;

namespace CakeBot.Helper.Modules.Profile
{
    public class BaseProfile
    {
        private static readonly Point XpBarBorderLocation = BaseProfileBuilder.XpBarBorderLocation;
        private static string baseAddress = Directory.GetCurrentDirectory();
        private static List<FontFamily> _fontFamilies;
        private static Tuple<string, int> _username;
        private static ProfileXp _profileXp;
        private static UserStats _userStats;
        private static Dictionary<string, Size> _sizes;
        private static Dictionary<string, Font> _fonts;
        private static Dictionary<string, Rgba32> _colors;

        public async Task CreateForUser(IUser user, string saveLocation = @"\Images\temp.png") // TODO need to be dispose disposable objects
        {
            await Initialize(user);
            var builder = new BaseProfileBuilder
            {
                Background = await UserQueries.GetUserBackground(user.Id),
                BoxOpacity = 0.8f,
                NameBox = SetupNameBox(),
                InfoBox = SetupInfoBox(),
                InfoBoxBorder = DetermineInfoBoxBorder(_profileXp.Level),
                Avatar = SetupAvatar(user),
                AvatarBorder = DetermineAvatarBorder(user),
                XpBar = SetupXpBar(_profileXp),
                XpFont = _fonts["Xp"],
                LevelFont = _fonts["Level"],
                MainFont = _fonts["Main"],
                UsernameFont = _fonts["Username"],
                InfoFont = _fonts["Info"],
                ProfileXp = _profileXp,
                Username = _username.Item1,
                LevelText = "Level",
                XpBorderColor = _colors["XpBorder"],
                XpBorder = SetupXpBorder(_sizes["XpBarBorder"]),
                UserStats = _userStats,
                Globe = SetupGlobe(),
                RankFont = GetRankFont(_userStats.IsHighRank)
            };
            builder.XpBackground = SetupXpBackground(builder.XpBorder);
            builder.Build(saveLocation);
        }

        private Font GetRankFont(bool high)
        {
            if (high) return _fonts["HighRank"];
            return _fonts["Rank"];
        }

        private Rectangle SetupXpBorder(Size xpBarBorderSize)
        {
            var xpBorder = new Rectangle(XpBarBorderLocation, xpBarBorderSize);
            return xpBorder;
        }

        private Image<Rgba32> SetupGlobe()
        {
            return Image.Load(baseAddress + @"\Images\globe.png");
        }
        private Image<Rgba32> SetupXpBar(ProfileXp profileXp)
        {
            var xpBar = new Image<Rgba32>(profileXp.BarWidth + 1, 9);
            xpBar.Mutate(b => b.Fill(_colors["XpBar"]));
            return xpBar;
        }

        private Image<Rgba32> SetupXpBackground(Rectangle xpBorder)
        {
            var xpBackground = new Image<Rgba32>(xpBorder.Width, xpBorder.Height);
            xpBackground.Mutate(b => b.Fill(Rgba32.White));
            return xpBackground;
        }

        private Image<Rgba32> DetermineAvatarBorder(IUser user)
        {
            switch (user.Status)
            {
                case UserStatus.Online:
                    return Image.Load( baseAddress+@"\Images\avatar_border-active.png");
                case UserStatus.Idle:
                    return Image.Load( baseAddress+@"\Images\avatar_border-idle.png");
                case UserStatus.DoNotDisturb:
                    return Image.Load( baseAddress+@"\Images\avatar_border-disturb.png");
                case UserStatus.Offline:
                    return Image.Load( baseAddress+@"\Images\avatar_border-active.png"); //TODO replace
                case UserStatus.AFK:
                    return Image.Load( baseAddress+@"\Images\avatar_border-active.png"); //TODO replace
                case UserStatus.Invisible:
                    return Image.Load( baseAddress+@"\Images\avatar_border-active.png"); //TODO replace
                default:
                    return Image.Load( baseAddress+@"\Images\avatar_border-active.png");
            }
        }

        private Image<Rgba32> SetupAvatar(IUser user)
        {
            var wc = new WebClient();
            var bytes = wc.DownloadData(user.GetAvatarUrl());
            var ms = new MemoryStream(bytes);
            var avatar = Image.Load(ms);
            avatar.Mutate(i => i.Resize(new Size(75, 75)));
            return avatar;
        }

        private Image<Rgba32> DetermineInfoBoxBorder(int level)
        {
            switch (level)
            {
                case int n when (n >= 10 && n < 20):
                    return Image.Load( baseAddress+@"\Images\border-10.png");
                case int n when (n >= 20 && n < 30):
                    return Image.Load( baseAddress+@"\Images\border-20.png");
                case int n when (n >= 30 && n < 40):
                    return Image.Load( baseAddress+@"\Images\border-30.png");
                case int n when (n >= 40 && n < 50):
                    return Image.Load( baseAddress+@"\Images\border-40.png");
                case int n when (n >= 50):
                    return Image.Load( baseAddress+@"\Images\border-50.png");
                default:
                    return Image.Load( baseAddress+@"\Images\border-0.png");
            }
        }

        private Image<Rgba32> SetupInfoBox()
        {
            var infoBox = new Image<Rgba32>(281, 79);
            infoBox.Mutate(x => x.Fill(_colors["Box"]));
            return infoBox;
        }

        private Image<Rgba32> SetupNameBox()
        {
            var nameBox = new Image<Rgba32>(195, 48);
            nameBox.Mutate(x => x.Fill(_colors["Box"]));
            return nameBox;
        }

        private async Task Initialize(IUser user)
        {
            _fontFamilies = GetFontFamilies();
            _username = FitUsername(user.Username);
            _profileXp = await GetUserXp(user.Id);
            _sizes = SetupSizes();
            _fonts = SetupFonts(_fontFamilies, _username);
            _colors = SetupColors();
            _userStats = await GetUserStats(user.Id);
        }
        
        private List<FontFamily> GetFontFamilies()
        {
            var resultFonts = new List<FontFamily>();
            var fonts = new FontCollection();
            var basicFont = fonts.Install(baseAddress + @"\Fonts\zil.ttf");
            var japFont = fonts.Install(baseAddress + @"\Fonts\Koruri-Bold.ttf");
            resultFonts.Add(basicFont);
            resultFonts.Add(japFont);

            return resultFonts;
        }

        private Tuple<string, int> FitUsername(string username)
        {
            switch (username.Length)
            {
                case int n when (n <= 12):
                    return new Tuple<string, int>(username, 22);
                case int n when (n > 12 && n <= 17):
                    return new Tuple<string, int>(username, 20);
                case int n when (n > 17 && n <= 20):
                    return new Tuple<string, int>(username, 18);
                case int n when (n > 20):
                    return new Tuple<string, int>(username.Truncate(19) + "...", 18);
                default:
                    return new Tuple<string, int>(username, 22);
            }
        }

        private async Task<UserStats> GetUserStats(ulong userId)
        {
            var userStats = new UserStats();
            await userStats.SetData(userId);
            return userStats;
        }
        private async Task<ProfileXp> GetUserXp(ulong userId)
        {
            var profileXp = new ProfileXp();
            await profileXp.GetData(userId);

            return profileXp;
        }

        private Dictionary<string, Size> SetupSizes()
        {
            var resultSizes = new Dictionary<string, Size>
            {
                {"XpBarBorder", new Size(77, 11)}
            };

            return resultSizes;
        }

        private Dictionary<string, Font> SetupFonts(List<FontFamily> fontFamilies, Tuple<string, int> username)
        {
            var font = fontFamilies[0]; // TODO replace to support multiple font families when there are special characters used
            var usernamefont = fontFamilies[1];

            var resultFonts = new Dictionary<string, Font>
            {
                {"Username", usernamefont.CreateFont(username.Item2)},
                {"Main", font.CreateFont(22)},
                {"Level", font.CreateFont(45)},
                {"Xp", font.CreateFont(9)},
                {"Info", font.CreateFont(15)},
                {"Rank", font.CreateFont(18)},
                {"HighRank", font.CreateFont(22)}
            };

            return resultFonts;
        }
        
        private Dictionary<string, Rgba32> SetupColors()
        {
            var resultColors = new Dictionary<string, Rgba32>
            {
                {"Box", Rgba32.FromHex("#3e3e3e")},
                {"XpBorder", Rgba32.FromHex("#C0C0C0")},
                {"XpBar", Rgba32.FromHex("#4D6EBC")}
            };

            return resultColors;
        }
    }
}
