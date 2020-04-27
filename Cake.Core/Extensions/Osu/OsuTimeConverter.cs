using Cake.Json.CakeModels.Osu;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cake.Core.Extensions.Osu
{
    public class OsuTimeConverter
    {
        private static IEnumerable<string> GetTimeZoneOfISOCode(string countryCode) => TzdbDateTimeZoneSource.Default.ZoneLocations
                .Where(x => x.CountryCode == countryCode)
                .Select(x => x.ZoneId);

        public List<OsuJsonUserRecent> ConvertDateTime(string countryCode, List<OsuJsonUserRecent> scores)
        {
            var zoneIds = GetTimeZoneOfISOCode(countryCode);

            foreach
            return recent;
        }
    }
}
