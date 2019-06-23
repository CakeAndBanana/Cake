using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Cake.Json.CakeModels.Osu;

namespace Cake.Json.CakeBuilders.Osu
{
    public class OsuScoreBuilder : OsuJsonBaseBuilder<OsuJsonScore>
    {
        private readonly NumberFormatInfo Nfi = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = ".", CurrencySymbol = "" };

        public string BeatmapId; // b
        public string UserId; // u
        public string Mode; // m
        public string Mods; // mods
        public string Limit; // limit

        public List<OsuJsonScore> Execute()
        {
            var scoreArray = ExecuteJson(OsuApiRequest.Scores);
            scoreArray = ProcessJson(scoreArray);
            return scoreArray.ToList();
        }

        public OsuJsonScore[] ProcessJson(OsuJsonScore[] scoreArray)
        {
            foreach (var item in scoreArray)
            {
                item.rounded_score = item.score.ToString("C0", Nfi);

                switch (Mode)
                {
                    case "0":
                        item.calculated_accuracy = 100.0 * (6 * item.count300 + 2 * item.count100 + item.count50) /
                                                   (6 * (item.count50 + item.count100 + item.count300 +
                                                         item.countmiss));
                        break;
                    case "1":
                        item.calculated_accuracy = 100.0 * (2 * item.count300 + item.count100) /
                                                   (2 * (item.count300 + item.count100 + item.countmiss));
                        break;
                    case "2":
                        item.calculated_accuracy = 100.0 * (item.count300 + item.count100 + item.count50) /
                                                   (item.count300 + item.count100 + item.count50 + item.countkatu +
                                                    item.countmiss);
                        break;
                    case "3":
                        item.calculated_accuracy =
                            100.0 * (6 * item.countgeki + 6 * item.count300 + 4 * item.countkatu + 2 * item.count100 +
                                     item.count50) /
                            (6 * (item.count50 + item.count100 + item.count300 + item.countmiss + item.countgeki +
                                  item.countkatu));
                        break;
                }
            }

            return scoreArray;
        }

        public override string Build(StringBuilder urlBuilder)
        {
            if (!string.IsNullOrEmpty(BeatmapId))
            {
                urlBuilder.Append("&b=");
                urlBuilder.Append(BeatmapId);
            }

            if (!string.IsNullOrEmpty(UserId))
            {
                urlBuilder.Append("&u=");
                urlBuilder.Append(UserId);
            }

            if (!string.IsNullOrEmpty(Mode))
            {
                urlBuilder.Append("&m=");
                urlBuilder.Append(Mode);
            }

            if (!string.IsNullOrEmpty(Mods))
            {
                urlBuilder.Append("&mods=");
                urlBuilder.Append(Mods);
            }

            if (!string.IsNullOrEmpty(Limit))
            {
                urlBuilder.Append("&limit=");
                urlBuilder.Append(Limit);
            }

            return urlBuilder.ToString();
        }
    }
}
