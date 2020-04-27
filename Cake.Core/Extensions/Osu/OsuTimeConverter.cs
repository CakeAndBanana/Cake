using Cake.Json.CakeModels.Osu;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cake.Core.Extensions.Osu
{
    public class OsuTimeConverter
    {
        public static List<OsuJsonUserRecent> ConvertRecentScores(string countryCode, List<OsuJsonUserRecent> scores)
        {
            var zoneLocation = TzdbDateTimeZoneSource.Default.ZoneLocations
                .Where(x => x.CountryCode == countryCode).FirstOrDefault();

            foreach (var score in scores)
            {
                TimeZoneInfo cetInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
                DateTimeOffset cetTime = TimeZoneInfo.ConvertTime(score.date, cetInfo);
                score.date
                    .Subtract(cetTime.Offset)
                    .ToOffset(cetTime.Offset);
            }
            return scores;
        }

        public static List<OsuJsonUserBest> ConvertBestScores(string countryCode, List<OsuJsonUserBest> scores)
        {
            var zoneLocation = TzdbDateTimeZoneSource.Default.ZoneLocations
                .Where(x => x.CountryCode == countryCode).FirstOrDefault();

            foreach (var score in scores)
            {
                TimeZoneInfo cetInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
                score.date = TimeZoneInfo.ConvertTimeFromUtc(score.date, cetInfo);
            }
            return scores;
        }
    }
}
