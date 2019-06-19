using Cake.Core.Discord.Embed.Builder;
using CakeBot.Helper.Modules.Osu.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.Core.Discord.Embeds
{
    public class OsuModuleEmbeds
    {
        public static CakeEmbedBuilder ReturnSetAccountEmbed(OsuJsonUser user)
        {
            return new CakeEmbedBuilder(EmbedType.Success)
                .WithTitle("osu!")
                .WithDescription($"Successfully set your username to \n\n**{user.username}**")
                .WithThumbnailUrl(user.image) as CakeEmbedBuilder;
        }
    }
}
