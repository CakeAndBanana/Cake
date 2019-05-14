using System;
using System.Threading.Tasks;
using CakeBot.Helper;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using Tweetinvi.Core.Extensions;

namespace CakeBot.Modules.Services
{
    public class TwitterService : CustomBaseService
    {
        public async Task ListTwitter()
        {
            try
            {

                var posts = await TwitterQueries.ListTwitterPost(Module.Context.Channel.Id);
                if (!posts.IsNullOrEmpty())
                {
                    var embedList = new CakeEmbedBuilder();
                    embedList.WithTitle($"Twitter users linked to this channel");

                    foreach (var post in posts)
                    {
                        var user = TwitterRealtime.GetUserById(post.TwitterId);
                        var creator = Startup.GetClient().GetUser((ulong) post.UserId);
                        embedList.Description += $"{user.Name} (added by {creator.ToString()}) \n";
                        embedList.WithThumbnailUrl(user.ProfileImageUrl);
                    }

                    embedList.WithFooter($"{posts.Count} user(s) out of 3 users");

                    await SendEmbedAsync(embedList);
                }
                else
                {
                    await SendMessageAsync("`No Twitter users found in this channel`");
                }

            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public async Task AddTwitter(string usertag)
        {
            try
            {
                var user = TwitterRealtime.GetUserByTag(usertag);
                if (user != null)
                {
                    var posts = await TwitterQueries.ListTwitterPost(Module.Context.Channel.Id);
                    if (posts.Count < 3)
                    {
                        await TwitterRealtime.AddId(Module.Context.User.Id,
                            user.Id,
                            Module.Context.Channel.Id,
                            Module.Context.Guild.Id);
                        await TwitterRealtime.ToggleStream();
                        await SendMessageAsync("`Twitter user added to this channel`");
                    }
                    else
                    {
                        await SendMessageAsync("`Maximum Twitter users reached in channel`");
                    }
                }
                else
                {
                    await SendMessageAsync("`Twitter user doesn't exist, check your input.`");
                }
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public async Task RemoveTwitter(string usertag)
        {
            try
            {
                var user = TwitterRealtime.GetUserByTag(usertag);
                if (user != null)
                {
                    var posts = TwitterQueries.ListTwitterPost(Module.Context.Channel.Id).Result
                        .FindAll(t => t.TwitterId == user.Id);
                    if (posts.Count > 0)
                    {
                        await TwitterRealtime.RemoveId(Module.Context.User.Id,
                            user.Id,
                            Module.Context.Channel.Id,
                            Module.Context.Guild.Id);
                        await TwitterRealtime.ToggleStream();
                        await SendMessageAsync("`Twitter user removed from this channel`");
                    }
                    else
                    {
                        await SendMessageAsync(
                            "`This twitter user is not found inside this channel or already has been removed.`");
                    }
                }
                else
                {
                    await SendMessageAsync("`Twitter user doesn't exist, check your input.`");
                }
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public async Task StatusTwitter()
        {
            await SendMessageAsync(
                $"`Status : {TwitterRealtime.GetStatus()}`");
        }
    }
}
