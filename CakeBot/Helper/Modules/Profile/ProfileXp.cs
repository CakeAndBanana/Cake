using System.Threading.Tasks;
using CakeBot.Helper.Database.Queries;
using SixLabors.Primitives;

namespace CakeBot.Helper.Modules.Profile
{
    public class ProfileXp
    {
        public int Level { get; set; }
        public int CurrentXp { get; set; }
        public int NextLevelXp { get; set; }
        public int BarWidth { get; set; }

        public async Task GetData(ulong userId)
        {
            await GetDatabaseData(userId);
            NextLevelXp = GetNextLevelXp(Level);
            BarWidth = CalculateXpBarWidth(CurrentXp, NextLevelXp);
        }

        private async Task GetDatabaseData(ulong userId)
        {
            Level = await UserQueries.GetUserLevel(userId);
            CurrentXp = await UserQueries.GetUserXp(userId);
        }

        private int GetNextLevelXp(int currentLevel)
        {
            int neededXp;
            if (Level == 0)
            {
                neededXp = 65;
            }
            else
            {
                neededXp = (int)(Global.BaseXp * (currentLevel * 1.45));
            }

            return neededXp;
        }

        private int CalculateXpBarWidth(int currentXp, int nextLevelXp, double xpBarWidth = 0.73)
        {
            var percentageFilled = (double) currentXp / nextLevelXp * 100;
            return (int)(xpBarWidth * percentageFilled);
        }
        public Point GetLevelLocation()
        {
            if (Level > 9) return new Point(235, 244);
            return new Point(243, 244);
        }
    }
}
