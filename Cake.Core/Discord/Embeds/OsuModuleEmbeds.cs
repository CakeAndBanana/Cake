using Cake.Core.Discord.Embed.Builder;
using Cake.Json.CakeModels.Osu;
using System;
using System.Collections.Generic;

namespace Cake.Core.Discord.Embeds
{
    public enum OsuModeEnum
    {
        osu,
        Taiko,
        Catch,
        Mania
    }

    public class OsuModuleEmbeds
    {
        public static CakeEmbedBuilder ReturnSetAccountEmbed(OsuJsonUser user)
        {
            return new CakeEmbedBuilder(EmbedType.Success)
                .WithTitle("osu!")
                .WithDescription($"Successfully set your username to \n\n**{user.username}**")
                .WithThumbnailUrl(user.image) as CakeEmbedBuilder;
        }


        public static CakeEmbedBuilder ReturnUserProfile(OsuJsonUser user, int mode)
        {
            var embedProfile = new CakeEmbedBuilder();
            embedProfile.WithAuthor(author =>
            {
                author
                    .WithName($"User stats of { user.username }")
                    .WithUrl($"{ user.url }")
                    .WithIconUrl($"{ user.flag }");
            })
                .WithThumbnailUrl($"https://a.ppy.sh/{user.user_id}_1512645682.png")
                .WithFooter($"{ (OsuModeEnum)mode }")
                .WithDescription(
                    $"**Ranked Score: **{ user.ranked_score }\n" +
                    $"**Total Score: **{ user.total_score }\n" +
                    $"**Rank: **{ user.pp_rank } ({ user.country } : { user.pp_country_rank })\n" +
                    $"**PP: **{ Math.Round(user.pp_raw, 1) }\n" +
                    $"**Accuracy: **{ user.accuracy }%\n" +
                    $"**Play Count: **{ user.playcount }\n" +
                    $"**Level: **{ user.level }\n" +
                    $"**Creation Date: ** { user.join_date.ToShortDateString() }")
                .AddField("Links",
                    $"[osu!chan]({ user.osuchan }) [osu!Skills]({ user.osuskills }) [osu!track]({ user.osutrack })\n");
            return embedProfile;
        }

        public static CakeEmbedBuilder ReturnUserBest(OsuJsonUser user, string thumbnail, List<Tuple<string, string>> fields, int mode)
        {
            var embedTop = new CakeEmbedBuilder();
            embedTop.WithAuthor(author =>
            {
                author
                    .WithName($"Top play(s) of {user.username}")
                    .WithUrl($"{user.url}")
                    .WithIconUrl($"{user.image}");
            })
            .WithFooter($"{(OsuModeEnum)mode}")
            .WithThumbnailUrl(thumbnail);

            //data of fields.
            foreach (var field in fields)
            {
                embedTop.AddField(x =>
                {
                    x.Name = field.Item1;
                    x.Value = field.Item2;
                });
            }

            return embedTop;
        }

        private static CakeEmbedBuilder ReturnUserRecentBase(OsuJsonUser user)
        {
            return new CakeEmbedBuilder()
            .WithAuthor(author =>
            {
                author
                    .WithName($"Recently played by {user.username}")
                    .WithUrl($"{user.url}")
                    .WithIconUrl($"{user.image}");
            }) as CakeEmbedBuilder;
        }

        public static CakeEmbedBuilder ReturnUserRecentList(OsuJsonUser user, OsuJsonBeatmap beatmap, string description, int mode)
        {
            return ReturnUserRecentBase(user)
                .WithUrl(beatmap.beatmap_url)
                .WithThumbnailUrl(beatmap.thumbnail)
                .WithFooter($"{(OsuModeEnum)mode}")
                .WithDescription(description) as CakeEmbedBuilder;
        }

        public static CakeEmbedBuilder ReturnUserRecent(OsuJsonUser user, OsuJsonBeatmap beatmap, OsuJsonUserRecent recent, string description, int mode, int retryCount)
        {
            return ReturnUserRecentBase(user)
                .WithUrl($"{beatmap.beatmap_url}")
                .WithThumbnailUrl($"{beatmap.thumbnail}")
                .WithTimestamp(recent.date)
                .WithTitle($"{beatmap.complete_title} {Math.Round(recent.starrating, 2)}★")
                .WithFooter($"{(OsuModeEnum)mode} ⌑ Status: {beatmap.approved_string} ⌑ #{retryCount} Try")
                .WithDescription(description)
                as CakeEmbedBuilder;
        }
    }
}
