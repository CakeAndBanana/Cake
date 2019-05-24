﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using CakeBot.Core;
using CakeBot.Helper;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Logging;
using Discord;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using IUser = Tweetinvi.Models.IUser;
using User = Tweetinvi.User;

namespace CakeBot.Modules.Services
{
    public class TwitterRealtime
    {
        private static readonly CakeEntities Db = new CakeEntities();
        private static List<TwitterPost> _users = new List<TwitterPost>();
        private static readonly IFilteredStream Stream = Tweetinvi.Stream.CreateFilteredStream();

        private static IAuthenticatedUser _authenticatedUser;

        private static void SetupStream()
        {
            Stream.MatchingTweetReceived += async (sender, args) =>
            {
                var sendlist = new List<TwitterPost>();
                try
                {
                    sendlist = _users.FindAll(t => t.TwitterId == args.Tweet.CreatedBy.Id);
                    Logger.LogInfo(
                        $"Tweet received from {args.Tweet.CreatedBy.Name} | {args.Tweet.CreatedBy.IdStr}");
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }

                foreach (var user in sendlist)
                {
                    try
                    {
                        var embedTwitter = new CakeEmbedBuilder();
                        var server = BotUtil.GetGuild((ulong)user.DiscordChannel.GuildId);
                        var channel = server.GetTextChannel((ulong)user.ChannelId);

                        embedTwitter.WithAuthor(args.Tweet.CreatedBy.Name, args.Tweet.CreatedBy.ProfileImageUrl, args.Tweet.CreatedBy.Url)
                            .WithFooter(args.Tweet.CreatedBy.Name, args.Tweet.CreatedBy.ProfileImageUrl)
                            .WithTimestamp(DateTime.Now)
                            .WithThumbnailUrl("");

                        if (args.Tweet.Media.Count >= 1)
                        {
                            embedTwitter.WithDescription(args.Tweet.FullText)
                                .WithImageUrl(args.Tweet.Media.First().MediaURL);

                            await SendMessage(embedTwitter.Build(), (ulong)user.DiscordChannel.GuildId, (ulong)user.ChannelId);
                            Logger.LogInfo(
                                $"Tweet send to Guild {server.Name} and to Channel {channel.Name}");

                        }
                        else
                        {
                            embedTwitter.WithDescription(args.Tweet.FullText);

                            await SendMessage(embedTwitter.Build(), (ulong)user.DiscordChannel.GuildId, (ulong)user.ChannelId);
                            Logger.LogInfo(
                                $"Tweet send to Guild {server.Name} and to Channel {channel.Name}");
                        }

                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.Message);
                    }
                }
            };
        }

        private static async Task SendMessage(Embed message, ulong guildId, ulong channelId)
        {
            var server = BotUtil.GetGuild(guildId);
            var channel = server.GetTextChannel(channelId);

            await channel.SendMessageAsync("", false, message);
        }

        private static void FillUsers()
        {
            _users = Db.TwitterPosts.ToList();
            var friends = _authenticatedUser.GetFriends().ToList();

            foreach (var user in _users)
            {
                if (!friends.Exists(u => u.Id == user.TwitterId) || Stream.ContainsFollow(user.TwitterId))
                {
                    if (Stream.ContainsFollow(user.TwitterId))
                    {

                    }
                    else
                    {
                        _authenticatedUser.FollowUser(user.TwitterId);
                        Stream.AddFollow(user.TwitterId);
                    }
                }
                else
                {
                    Stream.AddFollow(user.TwitterId);
                }
            }
        }

        private static void Init()
        {
            Auth.SetCredentials(GetCredentials());
            _authenticatedUser = User.GetAuthenticatedUser();
            SetupStream();
        }

        private static TwitterCredentials GetCredentials()
        {
            var credentials = new TwitterCredentials
            {
                ConsumerKey = Config.TwitterConsumerKey,
                ConsumerSecret = Config.TwitterConsumerSecret,
                AccessToken = Config.TwitterAccessToken,
                AccessTokenSecret = Config.TwitterAccessTokenSecret
            };
            return credentials;
        }

        public static IUser GetUserByTag(string tag)
        {
            var credentials = GetCredentials();
            Auth.SetUserCredentials(credentials.ConsumerKey, credentials.ConsumerSecret, credentials.AccessToken, credentials.AccessTokenSecret);
            var result = User.GetUserFromScreenName(tag);
            return result;
        }

        public static IUser GetUserById(long id)
        {
            var credentials = GetCredentials();
            Auth.SetUserCredentials(credentials.ConsumerKey, credentials.ConsumerSecret, credentials.AccessToken,
                credentials.AccessTokenSecret);
            var result = User.GetUserFromId(id);
            return result;
        }

        public static void ToggleStream()
        {
            if (Stream.StreamState == StreamState.Running)
            {
                Stream.StopStream();
                Stream.Credentials = GetCredentials();
            }

            if (Stream.Credentials == null)
            {
                Init();
                Stream.Credentials = GetCredentials();
            }

            FillUsers();
            Stream.StartStreamMatchingAnyCondition();
        }

        public static string GetStatus()
        {
            return Stream.StreamState.ToString();
        }

        public static async Task AddId(ulong discordId, long twitterId, ulong channelId , ulong guildId)
        {
            await TwitterQueries.AddTwitterPost(discordId, twitterId, channelId, guildId);
        }

        public static async Task RemoveId(ulong discordId, long twitterId, ulong channelId, ulong guildId)
        {
            await TwitterQueries.RemoveTwitterPost(discordId, twitterId, channelId, guildId);
        }
    }
}
