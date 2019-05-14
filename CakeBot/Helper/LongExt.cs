namespace CakeBot.Helper
{
    public static class LongExt
    {
        public static string ToShortAmount(this long amount)
        {
            var shortAmount = amount.ToString();
            if (amount > 1000 && amount < 1000000)
            {
                shortAmount = amount / 100 + "k";
                shortAmount = shortAmount.Insert(shortAmount.Length - 2, ".");
            }
            else if (amount > 1000000 && amount < 1000000000)
            {
                shortAmount = amount / 100000 + "m";
                shortAmount = shortAmount.Insert(shortAmount.Length - 2, ".");
            }
            else if (amount > 1000000000)
            {
                shortAmount = amount / 1000000000 + "b";
                shortAmount = shortAmount.Insert(shortAmount.Length - 2, ".");
            }
            return shortAmount;
        }
    }
}