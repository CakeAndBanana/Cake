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
using static CakeBot.Helper.Modules.Osu.OsuUtil;

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

                GetCalculatedAccuracy(item, Mode);
                var data = new WebClient().DownloadData($"https://osu.ppy.sh/osu/{item.beatmap_id}");
                var beatmapDownload = Beatmap.Read(new StreamReader(new MemoryStream(data, false))) ?? throw new ArgumentNullException("Beatmap.Read(new StreamReader(new MemoryStream(data, false)))");
                var mods = (Mods)Convert.ToInt32(item.enabled_mods);
                var diff = new DiffCalc().Calc(beatmapDownload, mods);

                if (pp && Mode == "0")
                {
                    var rawPp = new PPv2(new PPv2Parameters(beatmapDownload, diff, new Accuracy(item.count300, item.count100, item.count50, item.countmiss).Value(), item.countmiss, item.maxcombo, mods));
                    item.nochokeaccuracy = new Accuracy(item.count300 + item.countmiss, item.count100, item.count50, 0).Value() * 100;
                    var nochokePp = new PPv2(new PPv2Parameters(beatmapDownload, diff, (item.nochokeaccuracy / 100), 0, diff.Beatmap.GetMaxCombo(), mods));
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

                item.counttotal = beatmapDownload.CountCircles + beatmapDownload.CountSliders + beatmapDownload.CountSpinners;

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
