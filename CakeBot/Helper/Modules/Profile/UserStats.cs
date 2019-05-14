using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Database.Queries;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;

namespace CakeBot.Helper.Modules.Profile
{
    public class UserStats
    {
        public string Balance { get; set; }
        public string TotalXp { get; set; }
        public string Casts { get; set; }
        public string RankString { get; set; }
        public int Rank { get; set; }
        public bool IsHighRank { get; set; } = false;
        public Rgba32 ColorHex { get; set; }// = Rgba32.FromHex("#0000FF");

        public async Task SetData(ulong userId)
        {
            Rank = await UserQueries.GetUserRank(userId);
            if (Rank < 10) IsHighRank = true;
            RankString = $"#{Rank.GetShortRank()}";
            if (Rank < 10000) RankString = $"# {Rank.GetShortRank()}";
            Balance = "€" + (await EconomyQueries.FindMoney(userId, new CakeBotEntities())).ToShortAmount();
            Casts = (await FishQueries.GetTotalCasts(userId)).ToShortAmount();
            TotalXp = (await UserQueries.GetTotalXp(userId)).ToShortAmount();
            ColorHex = Rgba32.FromHex("#" + await UserQueries.GetProfileColor(userId));
            
        }

        public Point GetRankLocation()
        {
            Point RankLocation = new Point(153, 258);
            Point Sub1kRankLocation = new Point(156, 258);
            Point Sub100RankLocation = new Point(159, 258);
            Point Sub10RankLocation = new Point(168, 258);
            if (Rank < 10) return Sub10RankLocation;
            else if (Rank < 100) return Sub100RankLocation;
            else if (Rank < 1000) return Sub1kRankLocation;
            else return RankLocation;
        }
    }
}
