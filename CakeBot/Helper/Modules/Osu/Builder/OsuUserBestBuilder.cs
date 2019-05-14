using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CakeBot.Helper.Modules.Osu.Model;
using OppaiSharp;
using Tweetinvi.Core.Extensions;

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
                    var data = new WebClient().DownloadData($"https://osu.ppy.sh/osu/{item.beatmap_id}");
                    var beatmapDownload = Beatmap.Read(new StreamReader(new MemoryStream(data, false))) ?? throw new ArgumentNullException("Beatmap.Read(new StreamReader(new MemoryStream(data, false)))");
                    var mods = (Mods)item.enabled_mods;
                    var diff = new DiffCalc().Calc(beatmapDownload, mods);
                    item.starrating = diff.Total;
                }

                play++;
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
