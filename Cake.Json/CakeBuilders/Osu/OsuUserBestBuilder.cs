using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cake.Json;
using CakeBot.Helper.Modules.Osu.Model;
using OppaiSharp;

namespace CakeBot.Helper.Modules.Osu.Builder
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
            osuJsonUserBestArray = ProcessJson(osuJsonUserBestArray);

            if (PlayNumber != null)
            {
                return osuJsonUserBestArray.ToList().FindAll(x => x.play_number == PlayNumber);
            }
            return (Recent ? osuJsonUserBestArray.OrderByDescending(x => x.date).Take(5).ToList() : osuJsonUserBestArray.ToList());
        }

        private OsuJsonUserBest[] ProcessJson(OsuJsonUserBest[] array)
        {
            var play = 1;
            foreach (var item in array)
            {
                item.play_number = play;
                OsuUtil.GetCalculatedAccuracy(item, Mode);

                if (item.enabled_mods > 0)
                {
                    //Star rating
                    var data = OsuDlBeatmap.FindMap(item.beatmap_id);
                    var beatmapData = Beatmap.Read(new StreamReader(new MemoryStream(data, false))) ?? throw new ArgumentNullException("Beatmap.Read(new StreamReader(new MemoryStream(data, false)))");
                    var diff = new DiffCalc().Calc(beatmapData, (Mods)item.enabled_mods);
                    item.starrating = diff.Total;
                }

                play++;
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

            if (Limit != null || Limit != "")
            {
                urlBuilder.Append("&limit=");
                urlBuilder.Append(Limit);
            }

            if (Type != null || Type != "")
            {
                urlBuilder.Append("&type=");
                urlBuilder.Append(Type);
            }

            return urlBuilder.ToString();
        }
    }
}
