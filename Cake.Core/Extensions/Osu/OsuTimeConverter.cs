using Cake.Json.CakeModels.Osu;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Linq;
using TimeZoneConverter;

namespace Cake.Core.Extensions.Osu
{
    public class OsuTimeConverter
    {
        private static TzdbZoneLocation GetTzdbZoneLocationFromCountryCode(string countryCode) => TzdbDateTimeZoneSource.Default.ZoneLocations
            .Where(x => x.CountryCode == countryCode).FirstOrDefault();

        private static TimeZoneInfo GetTimeZoneInfo(string countryCode) => TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(GetTzdbZoneLocationFromCountryCode(countryCode).ZoneId));

        public static List<T> ConvertScorableDate<T>(string countryCode, List<T> scores) where T: OsuJsonScorable
        {
            var timeZoneInfo = GetTimeZoneInfo(countryCode);

            foreach (var score in scores)
            {
                DateTimeOffset cetTime = TimeZoneInfo.ConvertTime(score.date.UtcDateTime, timeZoneInfo);
                score.date = score.date.AddHours(cetTime.Offset.TotalHours);
            }

            return scores;
        }
    }
}
