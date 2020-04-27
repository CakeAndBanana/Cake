using System;
using System.Reflection;

namespace Cake.Core.Extensions
{
    public class TimeFormat
    {
        public static string ToShortTimeSpan(TimeSpan time)
        {
            var monthsCalculation = ConvertionTupleDate(time.Days, 30);
            var yearsCalculation = ConvertionTupleDate(monthsCalculation.Item1, 12);
            return TimetoString(new Time(yearsCalculation.Item1, monthsCalculation.Item1, monthsCalculation.Item2, 0, 0));
        }

        public static string ToLongTimeSpan(TimeSpan time)
        {
            var monthsCalculation = ConvertionTupleDate(time.Days, 30);
            var yearsCalculation = ConvertionTupleDate(monthsCalculation.Item1, 12);
            return TimetoString(new Time(yearsCalculation.Item1, monthsCalculation.Item1, monthsCalculation.Item2, time.Hours, time.Minutes));
        }

        private static string TimetoString(Time model)
        {
            var output = "";
            var properties = model.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in properties)
            {
                var value = (int)propertyInfo.GetValue(model, null);
                string name = propertyInfo.Name;
                if (value == 1)
                {
                    name = name.Remove(name.Length - 1);
                }

                if (value != 0)
                {
                    output += $"{value} {name} ";
                }
            }
            return output;
        }

        private static Tuple<int, int> ConvertionTupleDate(int convertdate, int multipier)
        {
            int calcInt = 0;
            if (convertdate >= multipier)
            {
                while (true)
                {
                    calcInt++;
                    convertdate -= multipier;
                    if (convertdate < multipier)
                    {
                        break;
                    }
                }
            }
            return Tuple.Create(calcInt, convertdate);
        }

        private class Time
        {
            public int years { get; set; }
            public int months { get; set; }
            public int days { get; set; }
            public int hours { get; set; }
            public int minutes { get; set; }

            public Time(int years, int months, int days, int hours, int minutes)
            {
                this.years = years;
                this.months = months;
                this.days = days;
                this.hours = hours;
                this.minutes = minutes;
            }
        }
    }
}