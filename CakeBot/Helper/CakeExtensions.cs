using System.Collections.Generic;

namespace CakeBot.Helper
{
    public static class CakeExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static string GetShortRank(this int rank)
        {
            string shortRank = rank.ToString();
            if (rank > 100000 && rank < 1000000)
            {
                shortRank = rank / 100 + "k";
                shortRank = shortRank.Insert(shortRank.Length - 2, ".");
            }
            return shortRank;
        }
    }
}