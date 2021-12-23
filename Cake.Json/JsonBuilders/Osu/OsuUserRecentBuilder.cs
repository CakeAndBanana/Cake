using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cake.Json.CakeModels.Osu;
using OppaiSharp;

namespace Cake.Json.CakeBuilders.Osu
{
    public class OsuUserRecentBuilder : OsuJsonBaseBuilder<OsuJsonUserRecent>
    {
        private readonly NumberFormatInfo _nfi = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = ".", CurrencySymbol = "" };

        public int UserId; // u
        public int Mode; // m
        public string Limit; // limit (not used)
        public string Type; // type (not used)

        public List<OsuJsonUserRecent> Execute(bool retry = false)
        {
            var recentArray = ExecuteJson(OsuApiRequest.RecentPlayed);
            recentArray = ProcessJson(recentArray, retry);

            return recentArray.ToList();
        }

        private OsuJsonUserRecent[] ProcessJson(OsuJsonUserRecent[] array, bool retrycount)
        {
            foreach (var item in array)
            {
                if (retrycount) continue;

                //Get Beatmap last update
                var beatmapBuilder = new OsuBeatmapBuilder
                {
                    Mode = Mode,
                    ConvertedIncluded = "1",
                    BeatmapId = item.beatmap_id
                };

                item.Beatmap = beatmapBuilder.Execute().First();

                OsuUtil.GetCalculatedAccuracy(item, Mode);
                var data = OsuDlBeatmap.FindMap(item.beatmap_id, item.Beatmap.last_update.DateTime);
                var beatmapData = Beatmap.Read(new StreamReader(new MemoryStream(data, false)));
                var diff = new DiffCalc().Calc(beatmapData, (Mods)item.enabled_mods);

                var rawPp = new PPv2(new PPv2Parameters(beatmapData, diff, new Accuracy(item.count300, item.count100, item.count50, item.countmiss).Value(), item.countmiss, item.maxcombo, (Mods)item.enabled_mods));
                item.nochokeaccuracy = new Accuracy(item.count300 + item.countmiss, item.count100, item.count50, 0).Value() * 100;
                var nochokePp = new PPv2(new PPv2Parameters(beatmapData, diff, item.nochokeaccuracy / 100, 0, diff.Beatmap.GetMaxCombo(), (Mods)item.enabled_mods));
                item.pp = rawPp.Total;
                item.nochokepp = nochokePp.Total;

                item.rounded_score = item.score.ToString("C0", _nfi);

                if(item.maxcombo <= (beatmapData.GetMaxCombo() - (item.count100 + item.count50)) || item.rank == "XH" || item.rank == "SH")
                    item.choked = true;
                else if (item.countmiss > 0)
                    item.choked = true;

                item.starrating = diff.Total;

                item.counttotal = beatmapData.CountCircles + beatmapData.CountSliders + beatmapData.CountSpinners;

                item.rounded_score = item.score.ToString("C0", _nfi);

                item.standardhit = item.count300 + item.count100 + item.count50 + item.countmiss;

                item.hitted = item.countkatu + item.countgeki + item.count300 + item.count100 + item.count50 + item.countmiss;

                item.completion = item.standardhit / (double)item.counttotal * 100;
            }

            return array;
        }

        public override string Build(StringBuilder urlBuilder)
        {
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                urlBuilder.Append("&u=").Append(UserId);
            }

            if (!string.IsNullOrEmpty(Mode.ToString()))
            {
                urlBuilder.Append("&m=").Append(Mode);
            }

            if (!string.IsNullOrEmpty(Type))
            {
                urlBuilder.Append("&type=").Append(Type);
            }

            if (!string.IsNullOrEmpty(Limit))
            {
                urlBuilder.Append("&limit=").Append(Limit);
            }

            return urlBuilder.ToString();
        }
    }
}