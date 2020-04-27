using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cake.Json.CakeModels.Osu;

namespace Cake.Json.CakeBuilders.Osu
{
    public class OsuBeatmapBuilder : OsuJsonBaseBuilder<OsuJsonBeatmap>
    {
        public string Since; // since
        public string BeatmapSetId; // s
        public int? BeatmapId; // b
        public string UserId; // u
        public string Type; // type
        public string Mode; // m
        public string ConvertedIncluded; // a
        public string Hash; // h
        public string Limit; // limit

        public List<OsuJsonBeatmap> Execute()
        {
            var beatmapsArray = ExecuteJson(OsuApiRequest.Beatmap);

            beatmapsArray = ProcessJson(beatmapsArray);
            return beatmapsArray.ToList();
        }

        public OsuJsonBeatmap[] ProcessJson(OsuJsonBeatmap[] array)
        {
            foreach (var item in array)
            {
                item.complete_title = $"{item.artist} - {item.title} [{item.version}]";

                item.thumbnail = "https://b.ppy.sh/thumb/" + item.beatmapset_id + "l.jpg";

                if (BeatmapId != null || BeatmapSetId != null)
                {
                    if (BeatmapSetId != null)
                    {
                        item.beatmapset_url = "https://osu.ppy.sh/s/" + item.beatmapset_id; item.beatmap_url = "NULL";
                    }
                    else
                    {
                        item.beatmapset_url = "https://osu.ppy.sh/s/" + item.beatmapset_id; item.beatmap_url = "https://osu.ppy.sh/b/" + item.beatmap_id;
                    }
                }
                item.download = OsuUtil.OsuDownload + item.beatmapset_id;
                item.download_no_video = OsuUtil.OsuDownload + item.beatmapset_id + "n";
                item.osu_direct = OsuUtil.OsuDirect + item.beatmapset_id;
                item.bloodcat = OsuUtil.Bloodcat + item.beatmapset_id;
                switch (item.approved)
                {
                    case "-2":
                        item.approved_string = "graveyard";
                        break;
                    case "-1":
                        item.approved_string = "pending";
                        break;
                    case "1":
                        item.approved_string = "ranked";
                        break;
                    case "2":
                        item.approved_string = "approved";
                        break;
                    case "3":
                        item.approved_string = "qualified";
                        break;
                    case "4":
                        item.approved_string = "loved";
                        break;
                    default:
                        item.approved_string = "NULL";
                        break;
                }
            }

            return array;
        }

        public override string Build(StringBuilder urlBuilder)
        {
            if (!string.IsNullOrEmpty(Since))
            {
                urlBuilder.Append("&since=").Append(Since);
            }

            if (!string.IsNullOrEmpty(BeatmapSetId))
            {
                urlBuilder.Append("&s=").Append(BeatmapSetId);
            }

            if (BeatmapId != null)
            {
                urlBuilder.Append("&b=").Append(BeatmapId);
            }

            if (!string.IsNullOrEmpty(UserId))
            {
                urlBuilder.Append("&u=").Append(UserId);
            }

            if (!string.IsNullOrEmpty(Mode))
            {
                urlBuilder.Append("&m=").Append(Mode);
            }

            if (!string.IsNullOrEmpty(Type))
            {
                urlBuilder.Append("&type=").Append(Type);
            }

            if (!string.IsNullOrEmpty(ConvertedIncluded))
            {
                urlBuilder.Append("&a=").Append(ConvertedIncluded);
            }

            if (!string.IsNullOrEmpty(Hash))
            {
                urlBuilder.Append("&h=").Append(Hash);
            }

            if (!string.IsNullOrEmpty(Limit))
            {
                urlBuilder.Append("&limit=").Append(Limit);
            }

            return urlBuilder.ToString();
        }
    }
}
