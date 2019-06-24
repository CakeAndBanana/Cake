using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cake.Json.CakeModels.Osu;

namespace Cake.Json.CakeBuilders.Osu
{
    public class OsuBestBuilder : OsuJsonBaseBuilder<OsuJsonBest>
    {
        public string UserId; // u
        public string Mode; // m
        public string Limit; // limit
        public string Type; // type

        public List<OsuJsonBest> Execute()
        {
            var bestArray = ExecuteJson(OsuApiRequest.BestPerformance);
            bestArray = ProcessJsonRecentArray(bestArray);

            return bestArray.ToList();
        }

        public OsuJsonBest[] ProcessJsonRecentArray(OsuJsonBest[] bestArray)
        {
            foreach (var item in bestArray)
            {
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
                    default:
                        throw new InvalidOperationException("`Hitted unreachable code in switch statement (OsuBestBuilder)`");
                }
            }

            return bestArray;
        }

        public override string Build(StringBuilder urlBuilder)
        {
            if (!string.IsNullOrEmpty(UserId))
            {
                urlBuilder.Append("&u=").Append(UserId);
            }

            if (!string.IsNullOrEmpty(Mode))
            {
                urlBuilder.Append("&m=").Append(Mode);
            }

            if (!string.IsNullOrEmpty(Limit))
            {
                urlBuilder.Append("&limit=").Append(Limit);
            }

            if (!string.IsNullOrEmpty(Type))
            {
                urlBuilder.Append("&type=").Append(Type);
            }

            return urlBuilder.ToString();
        }
    }
}
