using System;

namespace CakeBot.Helper.Modules.BF4
{
    public class Bf4Helper
    {
        public static string RankToUrl(int Rank) => $"http://www.cakebot.org/resource/bf/r{Rank}.png";
        public static double Accuracy(double shotsFired, double shotsHit) => (shotsHit / shotsFired * 100);
        public static double KDR(double Kills, double Deaths) => (Kills / Deaths);
        public static string WLRatio(double Wins, double Loss)
        {
            double PlayedGames = Wins + Loss;
            return Math.Round((Wins / PlayedGames * 100), 1) + "%";
        }
        public static string TimePlayed(int TotalTimePlayed) => ($"{TimeSpan.FromSeconds(TotalTimePlayed).Days} day(s) {TimeSpan.FromSeconds(TotalTimePlayed).Hours}hrs {TimeSpan.FromSeconds(TotalTimePlayed).Minutes}min");
    }
}
