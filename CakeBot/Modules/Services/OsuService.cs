using System;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Helper;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using CakeBot.Helper.Modules.Osu;
using CakeBot.Helper.Modules.Osu.Builder;
using CakeBot.Helper.Modules.Osu.Model;
using CakeBot.Modules.Modules;
using Tweetinvi.Core.Extensions;

namespace CakeBot.Modules.Services
{
    public class OsuService : CustomBaseService
    {
        private async Task<OsuUser> GetDatabaseEntity(ulong discordId)
        {
            var databaseProfile = await OsuQueries.FindUser(discordId);
            try
            {
                if (databaseProfile == null)
                {
                    await SendMessageAsync(
                        "`No osu account has been linked to this discord account, use osu set or type in username or user id after command.`");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }

            return databaseProfile;
        }

        public async Task SetUserWithId(string id)
        {
            try
            {
                var discordId = Module.Context.User.Id;
                var user = GetJsonUser(id, false);

                //Create a new database entity or update the old one for the found profile
                await OsuQueries.CreateOrUpdateUser(discordId, Convert.ToInt32(user.user_id));

                var embedResult = new CakeEmbedBuilder(EmbedType.Success);
                embedResult.WithTitle("osu!")
                    .WithDescription($"Successfully set your username to \n\n**{user.username}**")
                    .WithThumbnailUrl(user.image);

                await SendEmbedAsync(embedResult);
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

        public async Task SetUser(string username)
        {
            try
            {
                var discordId = Module.Context.User.Id;
                var user = GetJsonUser(username, true);

                //Create a new database entity or update the old one for the found profile
                await OsuQueries.CreateOrUpdateUser(discordId, Convert.ToInt32(user.user_id));

                var embedResult = new CakeEmbedBuilder(EmbedType.Success);
                embedResult.WithTitle("osu!")
                    .WithDescription($"Successfully set your username to \n\n**{user.username}**")
                    .WithThumbnailUrl(user.image);

                await SendEmbedAsync(embedResult);
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

        public async Task SetMode(int mode)
        {
            try
            {
                await OsuQueries.SetUserMode(Module.Context.User.Id, mode);

                var embedResult = new CakeEmbedBuilder(EmbedType.Success);
                embedResult.WithTitle("osu!")
                    .WithDescription($"Successfully set your mode to \n\n**{(OsuModeEnum)mode}**");

                switch (mode)
                {
                    case 0:
                        embedResult.WithThumbnailUrl("https://Cake.s-ul.eu/E0mwl85b");
                        break;
                    case 1:
                        embedResult.WithThumbnailUrl("https://Cake.s-ul.eu/e3NwsiS7");
                        break;
                    case 2:
                        embedResult.WithThumbnailUrl("https://Cake.s-ul.eu/7pDuSReU");
                        break;
                    case 3:
                        embedResult.WithThumbnailUrl("https://Cake.s-ul.eu/NsbLFL7e");
                        break;
                }

                await SendEmbedAsync(embedResult);
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

        public async Task SetTrack(string v1, bool v2)
        {
            await Module.Context.Channel.SendMessageAsync($"`WIP issue#12`");
        }

        public async Task GetUser(string osuId, bool findWithUsername)
        {
            try
            {
                var databaseProfile = GetDatabaseEntity(Module.Context.User.Id).Result;
                var mode = databaseProfile.OsuMode;

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseProfile.OsuId.ToString();
                    findWithUsername = false;
                }

                var user = GetJsonUser(osuId, findWithUsername, mode);

                var embedResult = new CakeEmbedBuilder();
                embedResult.WithAuthor(author =>
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

                await SendEmbedAsync(embedResult);
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

        public async Task GetUserBest(string osuId, bool findWithUsername, bool recent, int? play)
        {
            try
            {
                var databaseProfile = GetDatabaseEntity(Module.Context.User.Id).Result;
                var mode = databaseProfile.OsuMode;

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseProfile.OsuId.ToString();
                    findWithUsername = false;
                }

                var user = GetJsonUser(osuId, findWithUsername, mode);

                var embedTop = new CakeEmbedBuilder();
                embedTop.WithAuthor(author =>
                {
                    author
                        .WithName($"Top plays of {user.username}")
                        .WithUrl($"{user.url}")
                        .WithIconUrl($"{user.image}");
                })
                .WithFooter($"{(OsuModeEnum)mode}");

                var bestBuilder = new OsuUserBestBuilder
                {
                    Mode = mode.ToString(),
                    Limit = (recent ? 100 : 5).ToString(),
                    UserId = user.user_id,
                    Recent = recent,
                    PlayNumber = play
                };

                var best = bestBuilder.Execute();

                foreach (var item in best)
                {
                    var beatmapBuilder = new OsuBeatmapBuilder
                    {
                        Mode = mode.ToString(),
                        ConvertedIncluded = "1",
                        BeatmapId = item.beatmap_id
                    };

                    var result = beatmapBuilder.Execute();

                    if (best.First() == item)
                    {
                        embedTop.WithThumbnailUrl($"https://b.ppy.sh/thumb/{result[0].beatmapset_id}l.jpg");
                    }

                    var dateTicks = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - item.date.Ticks);

                    var date = dateTicks.TotalDays > 30 ? TimeFormat.ToShortTimeSpan(dateTicks) : TimeFormat.ToLongTimeSpan(dateTicks);

                    double star_rating;

                    if (item.starrating == 0)
                        star_rating = result[0].difficultyrating;
                    else
                        star_rating = item.starrating;

                    embedTop.AddField(x =>
                    {
                        x.Name = $"#{item.play_number}: {result[0].complete_title} {OsuMods.Modnames(Convert.ToInt32(item.enabled_mods))} {Math.Round(star_rating, 2)}★";
                        x.Value = $"**PP:** {Math.Round(item.pp, 0)} " +
                                  $"**Rank:** {item.rank.LevelEmotes()} " +
                                  $"**Accuracy:** {Math.Round(item.calculated_accuracy, 2)}% " +
                                  $"**Combo:** {item.maxcombo}({result[0].max_combo}) \n" +
                                  $"{date} ago\n" +
                                  $"**Downloads:** [Beatmap]({result[0].beatmap_url})" +
                                  $"([no vid]({result[0].beatmap_url + "n"})) " +
                                  $"[Bloodcat]({result[0].bloodcat})\n";
                    });
                }

                await SendEmbedAsync(embedTop);
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

        public async Task GetUserRecent(string osuId, bool findWithUsername, int total = 1)
        {
            try
            {
                if (total > 5)
                {
                    throw new CakeException("Total max = 5");
                }
                var databaseProfile = GetDatabaseEntity(Module.Context.User.Id).Result;
                var mapId = 0;
                var info = "";
                var mode = databaseProfile.OsuMode;

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseProfile.OsuId.ToString();
                    findWithUsername = false;
                }

                var user = GetJsonUser(osuId, findWithUsername, mode);

                var embedRecent = new CakeEmbedBuilder();
                embedRecent.WithAuthor(author =>
                {
                    author
                        .WithName($"Recently played by {user.username}")
                        .WithUrl($"{user.url}")
                        .WithIconUrl($"{user.image}");
                });

                var recentBuilder = new OsuUserRecentBuilder
                {
                    Mode = mode.ToString(),
                    Limit = total.ToString(),
                    UserId = user.user_id
                };

                var recent = recentBuilder.Execute(true);
                var first = true;

                if (recent.Count == 0)
                {
                    throw new CakeException("No recently plays found");
                }

                for (var i = 0; i < recent.Count; i++)
                {
                    var t = recent[i];

                    var beatmapBuilder = new OsuBeatmapBuilder
                    {
                        Mode = mode.ToString(),
                        ConvertedIncluded = "1",
                        BeatmapId = t.beatmap_id
                    };

                    var beatmap = beatmapBuilder.Execute();

                    if (first)
                    {
                        embedRecent.WithThumbnailUrl($"{beatmap[0].thumbnail}");
                        first = false;
                    }

                    var fc = t.maxcombo >= (beatmap[0].max_combo - 15) && t.countmiss == 0;

                    if (total > 1)
                    {
                        embedRecent.WithUrl($"{beatmap[0].beatmap_url}")
                            .WithFooter($"{(OsuModeEnum)mode}");

                        info += $"#{i + 1} ♢ [{beatmap[0].complete_title}]({beatmap[0].beatmap_url}) {Math.Round(t.starrating, 2)}★\n" +
                                $"**{t.rounded_score} ♢ {t.rank.LevelEmotes()} ♢ {t.maxcombo}x({beatmap[0].max_combo}x) {OsuMods.Modnames(Convert.ToInt32(t.enabled_mods))}**\n" +
                                $"{OsuUtil.Emote300} {t.count300} ♢ {OsuUtil.Emote100} {t.count100} ♢ {OsuUtil.Emote50} {t.count50} ♢ {OsuUtil.EmoteX} {t.countmiss} ♢ {Math.Round(t.calculated_accuracy, 2)}%\n";
                        if (t.rank != "F")
                        {
                            if (fc)
                            {
                                info += $"{Math.Round(t.pp, 2)} PP\n\n";
                            }
                            else
                            {
                                info += $"{Math.Round(t.pp, 2)} PP ♢ {Math.Round(t.nochokepp, 2)} PP if FC ({Math.Round(t.nochokeaccuracy, 2)}%)\n\n";
                            }
                        }
                        else
                        {
                            info += $"{Math.Round(t.completion, 2)}% completed\n";
                        }

                        mapId = Convert.ToInt32(beatmap[0].beatmap_id);
                    }
                    else
                    {
                        var retryCount = CheckRetries.Tries(mode.ToString(), t.user_id, beatmap[0].beatmap_id);

                        embedRecent.WithUrl($"{beatmap[0].beatmap_url}")
                            .WithThumbnailUrl($"{beatmap[0].thumbnail}")
                            .WithTimestamp(t.date)
                            .WithTitle($"{beatmap[0].complete_title} {Math.Round(t.starrating, 2)}★")
                            .WithFooter($"{(OsuModeEnum)mode} ⌑ Status : {beatmap[0].approved_string} ⌑ #{retryCount} Try");

                        info = $"**{t.rounded_score} ♢ " +
                                      $"{t.rank.LevelEmotes()} ♢ {t.maxcombo}x*({beatmap[0].max_combo}x)*** {OsuMods.Modnames(Convert.ToInt32(t.enabled_mods))} \n " +
                                      $"{OsuUtil.Emote300} {t.count300} ♢ {OsuUtil.Emote100} {t.count100} ♢ {OsuUtil.Emote50} {t.count50} ♢ {OsuUtil.EmoteX} {t.countmiss} ♢ {Math.Round(t.calculated_accuracy, 2)}%\n";
                        if (t.rank == "F")
                        {
                            info += $"{Math.Round(t.completion, 2)}% completed";
                        }
                        else
                        {
                            if (fc)
                            {
                                info += $"{Math.Round(t.pp, 2)} PP\n\n";
                            }
                            else
                            {
                                info += $"{Math.Round(t.pp, 2)} PP ♢ {Math.Round(t.nochokepp, 2)} PP if FC ({Math.Round(t.nochokeaccuracy, 2)}%)\n\n";
                            }
                        }
                        mapId = Convert.ToInt32(beatmap[0].beatmap_id);
                    }
                }
                embedRecent.WithDescription(info);

                if (mapId != 0)
                {
                    OsuModule.SetMapId(mapId);
                }

                await SendEmbedAsync(embedRecent);
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

        public async Task GetCompare(string osuId, bool findWithUsername)
        {
            try
            {
                var databaseProfile = GetDatabaseEntity(Module.Context.User.Id).Result;
                var mode = databaseProfile.OsuMode;

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseProfile.OsuId.ToString();
                    findWithUsername = false;
                }

                var user = GetJsonUser(osuId, findWithUsername, mode);
                var info = "";
                var mapId = ChannelQueries.GetMapId(Module.Context.Channel.Id, Module.Context.Guild.Id).Result;

                if (mapId == 0)
                {
                    throw new CakeException("No map found");
                }

                var embedCompare = new CakeEmbedBuilder();

                embedCompare.WithAuthor(author =>
                {
                    author
                        .WithName($"Compare score(s) of {user.username}")
                        .WithUrl(user.url)
                        .WithIconUrl(user.image);
                });

                var scoreBuilder = new OsuScoreBuilder
                {
                    BeatmapId = mapId.ToString(),
                    Mode = mode.ToString(),
                    UserId = user.user_id
                };

                var beatMapBuilder = new OsuBeatmapBuilder
                {
                    Mode = mode.ToString(),
                    ConvertedIncluded = "1",
                    BeatmapId = mapId.ToString()
                };

                var beatMap = beatMapBuilder.Execute();

                embedCompare.WithThumbnailUrl($"{beatMap[0].thumbnail}")
                            .WithTitle($"{beatMap[0].complete_title}")
                            .WithUrl($"{beatMap[0].beatmap_url}")
                            .WithFooter($"{(OsuModeEnum)mode} | {beatMap[0].approved_string}");

                var score = scoreBuilder.Execute();

                if (score.Count == 0)
                {
                    throw new CakeException("No score(s) found");
                }

                foreach (var t in score)
                {
                    var modName = t.enabled_mods == "0" ? "Nomod" : OsuMods.Modnames(Convert.ToInt32(t.enabled_mods));

                    var dateTicks = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - t.date.Ticks);

                    var date = dateTicks.TotalDays > 60 ? TimeFormat.ToShortTimeSpan(dateTicks) : TimeFormat.ToLongTimeSpan(dateTicks);

                    info += $"***{modName}*** \n" +
                            $"   **PP:** {Math.Round(t.pp, 0)} " +
                            $"   **Rank:**{t.rank.LevelEmotes()} " +
                            $"   **Accuracy:** {Math.Round(t.calculated_accuracy, 2)}% " +
                            $"   **Combo:** {t.maxcombo}({beatMap[0].max_combo}) \n" +
                            $"   {OsuUtil.Emote300} {t.count300} | {OsuUtil.Emote100} {t.count100} | {OsuUtil.Emote50} {t.count50} | {OsuUtil.EmoteX} {t.countmiss}\n " +
                            $"   {date} ago\n" +
                            $"\n";
                }
                embedCompare.WithDescription(info);

                await SendEmbedAsync(embedCompare);
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

        private OsuJsonUser GetJsonUser(string osuId, bool findWithUsername, int mode = -1)
        {
            var userBuilder = new OsuUserBuilder
            {
                Mode = mode == -1 ? null : mode.ToString(),
                UserId = osuId,
                Type = findWithUsername ? "string" : "id"
            };
            return userBuilder.Execute();
        }
    }
}
