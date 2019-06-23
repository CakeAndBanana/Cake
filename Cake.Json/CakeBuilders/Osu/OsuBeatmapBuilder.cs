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
                        item.approved_string = "Graveyard";
                        break;
                    case "-1":
                        item.approved_string = "Pending";
                        break;
                    case "1":
                        item.approved_string = "Ranked";
                        break;
                    case "2":
                        item.approved_string = "Approved";
                        break;
                    case "3":
                        item.approved_string = "Qualified";
                        break;
                    case "4":
                        item.approved_string = "Loved";
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
            if (Since != null || Since != "")
            {
                urlBuilder.Append("&since=");
                urlBuilder.Append(Since);
            }

            if (BeatmapSetId != null || BeatmapSetId != "")
            {
                urlBuilder.Append("&s=");
                urlBuilder.Append(BeatmapSetId);
            }

            if (BeatmapId != null)
            {
                urlBuilder.Append("&b=");
                urlBuilder.Append(BeatmapId);
            }

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

            if (ConvertedIncluded != null || ConvertedIncluded != "")
            {
                urlBuilder.Append("&a=");
                urlBuilder.Append(ConvertedIncluded);
            }

            if (Hash != null || Hash != "")
            {
                urlBuilder.Append("&h=");
                urlBuilder.Append(Hash);
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
