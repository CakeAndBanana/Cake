using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cake.Json.CakeModels.Osu;
using OppaiSharp;

namespace Cake.Json.CakeBuilders.Osu
{
    public class OsuUserBestBuilder : OsuJsonBaseBuilder<OsuJsonUserBest>
    {
        public int UserId; // u
        public int Mode; // m
        public string Limit; // limit
        public string Type; // type
        public bool Recent; // recent best
        public int? PlayNumber; // number for play

        public List<OsuJsonUserBest> Execute()
        {
            var osuJsonUserBestArray = ExecuteJson(OsuApiRequest.BestPerformance);

            if (PlayNumber != null)
            {
                var list = ProcessSingle(osuJsonUserBestArray);
                return list;
            }

            osuJsonUserBestArray = ProcessJson(osuJsonUserBestArray);

            return Recent ? osuJsonUserBestArray.OrderByDescending(x => x.date).Take(5).ToList() : osuJsonUserBestArray.ToList();
        }

        private OsuJsonUserBest[] ProcessJson(OsuJsonUserBest[] array)
        {
            var oldarray = array;
            if (Recent)
            {
                array = array.OrderByDescending(x => x.date).Take(5).ToArray();
            }

            foreach (var item in array)
            {
                item.play_number = (Array.IndexOf(oldarray, item) + 1);

                OsuUtil.GetCalculatedAccuracy(item, Mode);
                //Get Beatmap last update
                item.Beatmap = GetBeatmap(item.beatmap_id);

                if (item.enabled_mods > 0)
                {
                    //Star rating
                    var data = OsuDlBeatmap.FindMap(item.beatmap_id, item.Beatmap.last_update.DateTime);
                    var diff = new DiffCalc().Calc(Beatmap.Read(new StreamReader(new MemoryStream(data, false))), (Mods)item.enabled_mods);
                    item.starrating = diff.Total;
                }
            }
            return array;
        }

        private List<OsuJsonUserBest> ProcessSingle(OsuJsonUserBest[] array)
        {
            List<OsuJsonUserBest> list = new List<OsuJsonUserBest>
                {
                    array[(int)PlayNumber - 1]
                };

            if (list[0].enabled_mods > 0)
            {
                var data = OsuDlBeatmap.FindMap(list[0].beatmap_id, list[0].Beatmap.last_update.DateTime);
                var diff = new DiffCalc().Calc(Beatmap.Read(new StreamReader(new MemoryStream(data, false))), (Mods)list[0].enabled_mods);
                list[0].starrating = diff.Total;
            }
            
            list[0].Beatmap = GetBeatmap(list[0].beatmap_id);
            return list;
        }
        private OsuJsonBeatmap GetBeatmap(int beatmap_id)
        {
            var beatmapBuilder = new OsuBeatmapBuilder
            {
                Mode = Mode,
                ConvertedIncluded = "1",
                BeatmapId = beatmap_id
            };

            return beatmapBuilder.Execute().First();
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
