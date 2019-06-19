using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cake.Json;
using CakeBot.Helper.Modules.Osu.Model;
using OppaiSharp;

namespace CakeBot.Helper.Modules.Osu.Builder
{
    public class OsuUserRecentBuilder : OsuJsonBaseBuilder<OsuJsonUserRecent>
    {
        private readonly NumberFormatInfo _nfi = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = ".", CurrencySymbol = "" };

        public string UserId; // u
        public string Mode; // m
        public string Limit; // limit
        public string Type; // type

        public List<OsuJsonUserRecent> Execute(bool pp = false, bool retry = false)
        {
            var recentArray = ExecuteJson(OsuApiRequest.RecentPlayed);
            recentArray = ProcessJson(recentArray, pp, retry);

            return recentArray.ToList();
        }

        private OsuJsonUserRecent[] ProcessJson(OsuJsonUserRecent[] array, bool pp, bool retrycount)
        {
            foreach (var item in array)
            {
                if (retrycount) continue;

                OsuUtil.GetCalculatedAccuracy(item, Mode);
                var data = OsuDlBeatmap.FindMap(item.beatmap_id);
                var beatmapData = Beatmap.Read(new StreamReader(new MemoryStream(data, false))) ?? throw new ArgumentNullException("Beatmap.Read(new StreamReader(new MemoryStream(data, false)))");
                var diff = new DiffCalc().Calc(beatmapData, (Mods)item.enabled_mods);

                if (pp && Mode == "0")
                {
                    var rawPp = new PPv2(new PPv2Parameters(beatmapData, diff, new Accuracy(item.count300, item.count100, item.count50, item.countmiss).Value(), item.countmiss, item.maxcombo, (Mods)item.enabled_mods));
                    item.nochokeaccuracy = new Accuracy(item.count300 + item.countmiss, item.count100, item.count50, 0).Value() * 100;
                    var nochokePp = new PPv2(new PPv2Parameters(beatmapData, diff, (item.nochokeaccuracy / 100), 0, diff.Beatmap.GetMaxCombo(), (Mods)item.enabled_mods));
                    item.pp = rawPp.Total;
                    item.nochokepp = nochokePp.Total;
                }
                else
                    item.pp = 0;

                item.date = new DateTimeOffset(item.date.DateTime, TimeSpan.Zero).AddHours(item.date.Offset.TotalHours);

                item.rounded_score = item.score.ToString("C0", _nfi);

                item.hitted = item.countkatu + item.countgeki + item.count300 + item.count100 + item.count50 +
                              item.countmiss;

                item.starrating = diff.Total;

                item.counttotal = beatmapData.CountCircles + beatmapData.CountSliders + beatmapData.CountSpinners;

                item.rounded_score = item.score.ToString("C0", _nfi);

                item.standardhit = item.count300 + item.count100 + item.count50 +
                                   item.countmiss;

                item.hitted = item.countkatu + item.countgeki + item.count300 + item.count100 + item.count50 +
                              item.countmiss;

                item.completion = ((double)(item.standardhit) / (double)(item.counttotal) * 100);
            }

            return array;
        }

        public override string Build(StringBuilder urlBuilder)
        {
            if (UserId != null || UserId != "")
            {
                urlBuilder.Append("&u=");
                urlBuilder.Append(UserId);
            }

            if (Mode != null || Mode != "")
            {
                urlBuilder.Append("&m=");
                urlBuilder.Append(Mode);
            }

            if (Type != null || Type != "")
            {
                urlBuilder.Append("&type=");
                urlBuilder.Append(Type);
            }

            if (Limit != null || Limit != "")
            {
                urlBuilder.Append("&limit=");
                urlBuilder.Append(Limit);
            }

            return urlBuilder.ToString();
        }
    }
}
