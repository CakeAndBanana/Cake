using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Exceptions;
using Tweetinvi.Core.Extensions;

namespace CakeBot.Helper.Database.Queries
{
    public class TwitterQueries
    {
        private static readonly CakeBotEntities Db = new CakeBotEntities();

        public static async Task<List<TwitterPost>> ListTwitterPost(ulong channelId)
        {
            var result =
                await (from u in Db.TwitterPosts
                    where u.ChannelId == (long)channelId
                    select u).ToListAsync();
            return result;
        }

        public static async Task AddTwitterPost(ulong discordId, long twitterId, ulong channelId, ulong guildId)
        {
            //Find Channel
            var channel = await ChannelQueries.FindChannel(channelId);
            if (channel == null)
            {
                var guild = await GuildQueries.FindGuild(guildId);
                if (guild == null)
                {
                    await GuildQueries.CreateGuild(guildId);
                }
                await ChannelQueries.CreateChannel(channelId, guildId);
            }

            //Find TwitterPost
            var databaseProfile = await (from tp in Db.TwitterPosts
                where tp.TwitterId == twitterId && tp.ChannelId == (long)channelId
                select tp).ToListAsync();

            if (databaseProfile.IsEmpty())
            {
                var newpost = new TwitterPost
                {
                    TwitterId = twitterId,
                    ChannelId = (long)channelId,
                    UserId = (long)discordId
                };
                Db.TwitterPosts.Add(newpost);
                await Db.SaveChangesAsync();
            }
            else
            {
                throw new CakeException("Already in this channel");
            }
        }

        public static async Task RemoveTwitterPost(ulong discordId, long twitterId, ulong channelId, ulong guildId)
        {
            //Find Channel
            var channel = await ChannelQueries.FindChannel(channelId);
            if (channel == null)
            {
                var guild = await GuildQueries.FindGuild(guildId);
                if (guild == null)
                {
                    await GuildQueries.CreateGuild(guildId);
                }
                await ChannelQueries.CreateChannel(channelId, guildId);
            }

            //Find TwitterPost
            var posts = await (from tp in Db.TwitterPosts
                where tp.TwitterId == twitterId && tp.ChannelId == (long)channelId
                select tp).ToListAsync();

            if (posts.IsEmpty())
            {
                throw new CakeException("Post not found");
            }

            Db.TwitterPosts.RemoveRange(posts);
            await Db.SaveChangesAsync();
        }
    }
}
