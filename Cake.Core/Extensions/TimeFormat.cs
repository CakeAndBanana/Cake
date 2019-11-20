using System;

namespace Cake.Core.Extensions
{
    public class TimeFormat
    {
        protected TimeFormat()
        {
        }

        public static string ToShortTimeSpan(TimeSpan time)
        {
            var timestamp = "";
            var year = 0;
            var month = 0;
            var day = time.Days;

            if (day >= 30)
            {
                while (true)
                {
                    month++;
                    day -= 30;
                    if (day < 30)
                    {
                        break;
                    }
                }
            }

            if (month >= 12)
            {
                while (true)
                {
                    year++;
                    month -= 12;
                    if (month < 12)
                    {
                        break;
                    }
                }
            }

            if (year > 1)
            {
                timestamp += $"{year} years ";
            }
            if (year == 1)
            {
                timestamp += $"{year} year ";
            }
            if (month > 1)
            {
                timestamp += $"{month} months ";
            }
            if (month == 1)
            {
                timestamp += $"{month} month ";
            }
            if (day > 1)
            {
                timestamp += $"and {day} days ";
            }
            if (day == 1)
            {
                timestamp += $"and {day} day ";
            }

            return timestamp;
        }

        public static string ToLongTimeSpan(TimeSpan time)
        {
            var timestamp = "";
            var year = 0;
            var month = 0;
            var day = time.Days;
            var hour = time.Hours;
            var minute = time.Minutes;

            if (day >= 30)
            {
                while (true)
                {
                    month++;
                    day -= 30;
                    if (day < 30)
                    {
                        break;
                    }
                }
            }

            if (month >= 12)
            {
                while (true)
                {
                    month++;
                    month -= 12;
                    if (month < 12)
                    {
                        break;
                    }
                }
            }

            if (year > 1)
            {
                timestamp += $"{year} years ";
            }
            if (year == 1)
            {
                timestamp += $"{year} year ";
            }
            if (month > 1)
            {
                timestamp += $"{month} months ";
            }
            if (month == 1)
            {
                timestamp += $"{month} month ";
            }
            if (day > 1)
            {
                timestamp += $"{day} days ";
            }
            if (day == 1)
            {
                timestamp += $"{day} day ";
            }
            if (hour == 1)
            {
                timestamp += $"{hour} hour ";
            }
            if (hour > 1)
            {
                timestamp += $"{hour} hours ";
            }
            if (minute == 1)
            {
                timestamp += $"and {minute} minute ";
            }
            if (minute > 1)
            {
                timestamp += $"and {minute} minutes ";
            }

            return timestamp;
        }
    }
}