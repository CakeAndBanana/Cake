using System;
using System.Reflection;

namespace Cake.Core.Extensions
{
    internal class TimeFormat
    {
        public TimeSpan time { get; set; }
        public TimeFormat(TimeSpan time)
        {
            this.time = time;
        }

        public string ToShortString()
        {
            var monthsCalculation = ConvertionTupleDate(time.Days, 30);
            var yearsCalculation = ConvertionTupleDate(monthsCalculation.Item1, 12);
            return TimetoString(new Time(yearsCalculation.Item1, yearsCalculation.Item2, monthsCalculation.Item2, 0, 0));
        }

        public string ToLongString()
        {
            var monthsCalculation = ConvertionTupleDate(time.Days, 30);
            return TimetoString(new Time(0, monthsCalculation.Item1, monthsCalculation.Item2, time.Hours, time.Minutes));
        }

        private string TimetoString(Time model)
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

        internal Tuple<int, int> ConvertionTupleDate(int convertdate, int multipier)
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