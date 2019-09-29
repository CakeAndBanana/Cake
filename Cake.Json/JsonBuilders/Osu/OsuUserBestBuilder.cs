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
        public string UserId; // u
        public string Mode; // m
        public string Limit; // limit
        public string Type; // type
        public bool Recent; // recent best
        public int? PlayNumber; // number for play

        public List<OsuJsonUserBest> Execute()
        {
            var osuJsonUserBestArray = ExecuteJson(OsuApiRequest.BestPerformance);

            if (PlayNumber != null)
            {
                var beatmapid = osuJsonUserBestArray[0].beatmap_id;
                return osuJsonUserBestArray.ToList().TakeWhile(c => c.beatmap_id == beatmapid).ToList();
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

                if (item.enabled_mods > 0)
                {
                    //Star rating
                    var data = OsuDlBeatmap.FindMap(item.beatmap_id);
                    var diff = new DiffCalc().Calc(Beatmap.Read(new StreamReader(new MemoryStream(data, false))), (Mods)item.enabled_mods);
                    item.starrating = diff.Total;
                }
            }
            return array;
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
