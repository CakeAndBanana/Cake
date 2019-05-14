using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CakeBot.Helper.Modules.Osu.Model;
using OppaiSharp;
using Tweetinvi.Core.Extensions;

namespace CakeBot.Helper.Modules.Osu.Builder
{
    public class OsuBestBuilder : OsuJsonBaseBuilder<OsuJsonBest>
    {
        private static readonly NumberFormatInfo Nfi = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = ".", CurrencySymbol = "" };

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
                }
            }

            return bestArray;
        }

        public override string Build(StringBuilder urlBuilder)
        {
            if (!UserId.IsNullOrEmpty())
            {
                urlBuilder.Append("&u=");
                urlBuilder.Append(UserId);
            }

            if (!Mode.IsNullOrEmpty())
            {
                urlBuilder.Append("&m=");
                urlBuilder.Append(Mode);
            }

            if (!Limit.IsNullOrEmpty())
            {
                urlBuilder.Append("&limit=");
                urlBuilder.Append(Limit);
            }
            if (!Type.IsNullOrEmpty())
            {
                urlBuilder.Append("&type=");
                urlBuilder.Append(Type);
            }

            return urlBuilder.ToString();
        }
    }
}
