using System.Collections.Generic;
using System.Linq;
using System.Text;
using CakeBot.Helper.Modules.Osu.Model;
using Tweetinvi.Core.Extensions;

namespace CakeBot.Helper.Modules.Osu.Builder
{
    public class OsuBeatmapBuilder : OsuJsonBaseBuilder<OsuJsonBeatmap>
    {
        public string Since; // since
        public string BeatmapSetId; // s
        public string BeatmapId; // b
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
            if (!Since.IsNullOrEmpty())
            {
                urlBuilder.Append("&since=");
                urlBuilder.Append(Since);
            }

            if (!BeatmapSetId.IsNullOrEmpty())
            {
                urlBuilder.Append("&s=");
                urlBuilder.Append(BeatmapSetId);
            }

            if (!BeatmapId.IsNullOrEmpty())
            {
                urlBuilder.Append("&b=");
                urlBuilder.Append(BeatmapId);
            }

            if (!UserId.IsNullOrEmpty())
            {
                urlBuilder.Append("&u=");
                urlBuilder.Append(UserId);
            }

            if (!Type.IsNullOrEmpty())
            {
                urlBuilder.Append("&type=");
                urlBuilder.Append(Type);
            }

            if (!Mode.IsNullOrEmpty())
            {
                urlBuilder.Append("&m=");
                urlBuilder.Append(Mode);
            }

            if (!ConvertedIncluded.IsNullOrEmpty())
            {
                urlBuilder.Append("&a=");
                urlBuilder.Append(ConvertedIncluded);
            }

            if (!Hash.IsNullOrEmpty())
            {
                urlBuilder.Append("&h=");
                urlBuilder.Append(Hash);
            }

            if (!Limit.IsNullOrEmpty())
            {
                urlBuilder.Append("&limit=");
                urlBuilder.Append(Limit);
            }

            return urlBuilder.ToString();
        }
    }
}
