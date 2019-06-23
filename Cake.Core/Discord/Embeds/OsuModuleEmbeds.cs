using Cake.Core.Discord.Embed.Builder;
using Cake.Json.CakeModels.Osu;
using System;

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
    }
}
