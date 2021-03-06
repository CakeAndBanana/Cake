﻿using Cake.Core.Discord.Embed.Builder;
using Cake.Core.Exceptions;
using Cake.Core.Extensions.Osu;
using Cake.Json.CakeModels.Osu;
using System;
using System.Collections.Generic;

namespace Cake.Core.Discord.Embeds
{
    public enum OsuMapEnum
    {
        Loved = 4,
        Qualified = 3,
        Approved = 2,
        Ranked = 1,
        Pending = 0,
        WIP = -1,
        Graveyard = -2
    }

    public enum OsuModeEnum
    {
        osu,
        Taiko,
        Catch,
        Mania
    }

    public class OsuModuleEmbeds
    {
        protected OsuModuleEmbeds()
        {
        }

        public static CakeEmbedBuilder ReturnSetAccountEmbed(OsuJsonUser user)
        {
            return new CakeEmbedBuilder(EmbedType.Success)
                .WithTitle("osu!")
                .WithDescription($"Successfully set your username to \n\n**{user.username}**")
                .WithThumbnailUrl(user.image) as CakeEmbedBuilder;
        }

        public static CakeEmbedBuilder ReturnSetModeEmbed(int mode)
        {
            var embedResult = new CakeEmbedBuilder(EmbedType.Success);
            embedResult.WithTitle("osu!")
                .WithDescription($"Successfully set your mode to \n\n**{(OsuModeEnum)mode}**");

            switch (mode)
            {
                case 0:
                    embedResult.WithThumbnailUrl("http://cakebot.org/resource/osu/osu.png");
                    break;
                case 1:
                    embedResult.WithThumbnailUrl("http://cakebot.org/resource/osu/taiko.png");
                    break;
                case 2:
                    embedResult.WithThumbnailUrl("http://cakebot.org/resource/osu/ctb.png");
                    break;
                case 3:
                    embedResult.WithThumbnailUrl("http://cakebot.org/resource/osu/mania.png");
                    break;
                default:
                    throw new CakeException("Unexpected value in SetMode");
            }
            return embedResult;
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
                    .WithUrl(user.url)
                    .WithIconUrl(user.image);
            }) as CakeEmbedBuilder;
        }

        public static CakeEmbedBuilder ReturnUserRecent(OsuJsonUser user, OsuJsonBeatmap beatmap, OsuJsonUserRecent recent, string description, int mode, int retryCount)
        {
            return ReturnUserRecentBase(user)
                .WithUrl(beatmap.beatmap_url)
                .WithThumbnailUrl(beatmap.thumbnail)
                .WithTimestamp(recent.date)
                .WithTitle($"{beatmap.complete_title} {Math.Round(recent.starrating, 2)}★")
                .WithFooter($"{OsuMode.GetOfficialName(mode)} ⌑ {GetNameofApproved(beatmap.approved)} ⌑ #{retryCount} try")
                .WithDescription(description)
                as CakeEmbedBuilder;
        }

        public static CakeEmbedBuilder ReturnChannelCompare(OsuJsonUser user, OsuJsonBeatmap beatmap,string description, int mode)
        {
            return new CakeEmbedBuilder()
                .WithAuthor(author =>
                {
                author
                    .WithName($"Compare score(s) of {user.username}")
                    .WithUrl(user.url)
                    .WithIconUrl(user.image);
                })
                .WithThumbnailUrl(beatmap.thumbnail)
                .WithTitle(beatmap.complete_title)
                .WithUrl(beatmap.beatmap_url)
                .WithDescription(description)
                .WithFooter($"{OsuMode.GetOfficialName(mode)} ⌑ {GetNameofApproved(beatmap.approved)}") as CakeEmbedBuilder;
        }

        private static string GetNameofApproved(int approved)
        {
            var mapApproved = (OsuMapEnum)approved;

            return mapApproved.ToString();
        }
    }
}
