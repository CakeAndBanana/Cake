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
        public static List<OsuJsonUserRecent> ConvertRecentScores(string countryCode, List<OsuJsonUserRecent> scores)
        {
            var timeZoneInfo = GetTimeZoneInfo(countryCode);

            foreach (var score in scores)
            {
                DateTimeOffset cetTime = TimeZoneInfo.ConvertTime(score.date, timeZoneInfo);
                score.date = score.date.AddHours(cetTime.Offset.TotalHours);
            }

            return scores;
        }

        public static List<OsuJsonUserBest> ConvertBestScores(string countryCode, List<OsuJsonUserBest> scores)
        {
            var timeZoneInfo = GetTimeZoneInfo(countryCode);

            foreach (var score in scores)
            {
                DateTimeOffset cetTime = TimeZoneInfo.ConvertTime(score.date, timeZoneInfo);
                score.date = score.date.AddHours(cetTime.Offset.TotalHours);
            }

            return scores;
        }
    }
}
