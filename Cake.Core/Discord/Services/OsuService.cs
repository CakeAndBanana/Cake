using Cake.Core.Discord.Modules;
using Cake.Core.Exceptions;
using Cake.Core.Extensions;
using Cake.Core.Logging;
using Cake.Database.Models;
using Cake.Json.CakeBuilders.Osu;
using Cake.Json.CakeModels.Osu;
using LinqToDB.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Services
{
    public class OsuService : CustomBaseService
    {
        private async Task<CakeUser> GetDatabaseEntityAsync(ulong discordId)
        {
            var databaseProfile = await Database.Queries.UserQueries.FindOrCreateUser(discordId);

            try
            {
                if (databaseProfile.OsuId == 0)
                {
                    await SendMessageAsync("`No osu account has been linked to this discord account, use osu set (username / user id).`");
                }
            }
            catch (CakeException e)
            {
                Logger.Get().LogError(e);
            }

            return databaseProfile;
        }

        private OsuJsonUser GetJsonUser(string osuId, bool findWithUsername, int mode = -1)
        {
            var userBuilder = new OsuUserBuilder
            {
                Mode = mode == -1 ? null : mode.ToString(),
                UserId = osuId,
                Type = findWithUsername ? "string" : "id"
            };

            var user = userBuilder.Execute();

            if (user == null)
            {
                throw new CakeException("``User with given username or id is not found on osu!``");
            }

            return user;
        }

        public async Task SetAccount(string username)
        {
            var databaseprofile = await Database.Queries.UserQueries.FindOrCreateUser(Module.Context.User.Id).ConfigureAwait(false);
            var user = GetJsonUser(username, true);

            databaseprofile.OsuId = user.user_id;
            await Database.Queries.UserQueries.Update(databaseprofile);

            await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnSetAccountEmbed(user));
        }

        public async Task SetMode(int mode)
        {
            try
            {
                var databaseprofile = await Database.Queries.UserQueries.FindOrCreateUser(Module.Context.User.Id).ConfigureAwait(false);
                databaseprofile.OsuMode = mode;
                await Database.Queries.UserQueries.Update(databaseprofile);

                await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnSetModeEmbed(mode));
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
        }

        public async Task GetUserProfile(string osuId, bool findWithUsername)
        {
            try
            {
                var databaseUser = await GetDatabaseEntityAsync(Module.Context.User.Id).ConfigureAwait(false);
                var mode = databaseUser.OsuMode;

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseUser.OsuId.ToString();
                    findWithUsername = false;
                }
                var user = GetJsonUser(osuId, findWithUsername, mode);

                await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnUserProfile(user, mode));
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
        }

        public async Task GetUserBest(string osuId, bool findWithUsername, bool recent, int? play)
        {
            try
            {
                string thumbnail = null;
                var fields = new List<Tuple<string, string>>();
                var databaseProfile = await GetDatabaseEntityAsync(Module.Context.User.Id).ConfigureAwait(false);
                var mode = databaseProfile.OsuMode;

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseProfile.OsuId.ToString();
                    findWithUsername = false;
                }

                var user = GetJsonUser(osuId, findWithUsername, mode);

                var bestBuilder = new OsuUserBestBuilder
                {
                    Mode = mode.ToString(),
                    Limit = (recent || play != null ? 100 : 5).ToString(),
                    UserId = user.user_id.ToString(),
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
                        thumbnail = $"https://b.ppy.sh/thumb/{result[0].beatmapset_id}l.jpg";
                    }
                    
                    if (play != null)
                    {
                        item.play_number = (int)play;
                    }

                    var dateTicks = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - item.date.Ticks);

                    var date = dateTicks.TotalDays > 30 ? TimeFormat.ToShortTimeSpan(dateTicks) : TimeFormat.ToLongTimeSpan(dateTicks);

                    var starRating = Math.Abs(item.starrating) <= 0 ? result[0].difficultyrating : item.starrating;

                    fields.Add(new Tuple<string, string>($"#{item.play_number}: {result[0].complete_title} {OsuMods.Modnames(Convert.ToInt32(item.enabled_mods))} {Math.Round(starRating, 2)}★",
                                  $@"⤷ **PP:** {Math.Round(item.pp, 0)} " +
                                  $"**Rank:** {item.rank.LevelEmotes()} " +
                                  $"**Combo:** {item.maxcombo}({result[0].max_combo}) \n" +
                                  $" {OsuEmoteCodes.Emote300} {item.count300} ♢ {OsuEmoteCodes.Emote100} {item.count100} ♢ {OsuEmoteCodes.Emote50} {item.count50} ♢ {OsuEmoteCodes.EmoteX} {item.countmiss} ♢ {Math.Round(item.calculated_accuracy, 2)}%\n" +
                                  $" **Downloads:** [Beatmap]({result[0].beatmap_url})" +
                                  $"([no vid]({result[0].beatmap_url + "n"})) " +
                                  $"[Bloodcat]({result[0].bloodcat})\n" +
                                  $" {date} ago\n"));
                }

                await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnUserBest(user, thumbnail, fields, mode));
            }
            catch (Exception e)
            {
                await SendMessageAsync(e.Message);
            }
        }

        public async Task GetUserRecent(string osuId, bool findWithUsername, int total = 1)
        {
            try
            {
                if (total > 5)
                {
                    throw new CakeException("`Total amount must be lower than 5`");
                }

                var databaseProfile = await GetDatabaseEntityAsync(Module.Context.User.Id).ConfigureAwait(false);
                var mapId = 0;
                var info = "";
                var mode = databaseProfile.OsuMode;

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseProfile.OsuId.ToString();
                    findWithUsername = false;
                }

                var user = GetJsonUser(osuId, findWithUsername, mode);

                var recentBuilder = new OsuUserRecentBuilder
                {
                    Mode = mode.ToString(),
                    Limit = total.ToString(),
                    UserId = user.user_id.ToString()
                };

                var recent = recentBuilder.Execute(true);
                var first = true;
                OsuJsonBeatmap firstBeatmap = null;
                OsuJsonUserRecent firstRecent = null;
                var retryCount = 0;

                if (recent.Count == 0)
                {
                    throw new CakeException("`No recent play(s) has been found`");
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
                        firstBeatmap = beatmap[0];
                        first = false;
                    }

                    var fc = t.maxcombo >= (beatmap[0].max_combo - 15) && t.countmiss == 0;

                    if (total > 1)
                    {
                        info += $"**#{i + 1}** ♢ [{beatmap[0].complete_title}]({beatmap[0].beatmap_url}) **{Math.Round(t.starrating, 2)}★**\n" +
                                $"⤷ **{t.rounded_score} ♢ {t.rank.LevelEmotes()} ♢ {t.maxcombo}x({beatmap[0].max_combo}x) {OsuMods.Modnames(Convert.ToInt32(t.enabled_mods))}**\n" +
                                $"  {OsuEmoteCodes.Emote300} {t.count300} ♢ {OsuEmoteCodes.Emote100} {t.count100} ♢ {OsuEmoteCodes.Emote50} {t.count50} ♢ {OsuEmoteCodes.EmoteX} {t.countmiss} ♢ {Math.Round(t.calculated_accuracy, 2)}%\n";

                        if (t.rank != "F")
                        {
                            if (fc)
                            {
                                info += $" **{Math.Round(t.pp, 2)} PP**\n\n";
                            }
                            else
                            {
                                info += $" **{Math.Round(t.pp, 2)} PP** ♢ {Math.Round(t.nochokepp, 2)} PP if FC ({Math.Round(t.nochokeaccuracy, 2)}%)\n\n";
                            }
                        }
                        else
                        {
                            info += $" {Math.Round(t.completion, 2)}% completed\n";
                        }

                        mapId = Convert.ToInt32(beatmap[0].beatmap_id);
                    }
                    else
                    {
                        firstRecent = t;
                        retryCount = OsuCheckRetries.Tries(mode.ToString(), t.user_id, beatmap[0].beatmap_id);

                        info = $"**{t.rounded_score} ♢ " +
                                      $"{t.rank.LevelEmotes()} ♢ {t.maxcombo}x*({beatmap[0].max_combo}x)*** {OsuMods.Modnames(Convert.ToInt32(t.enabled_mods))} \n " +
                                      $"{OsuEmoteCodes.Emote300} {t.count300} ♢ {OsuEmoteCodes.Emote100} {t.count100} ♢ {OsuEmoteCodes.Emote50} {t.count50} ♢ {OsuEmoteCodes.EmoteX} {t.countmiss} ♢ {Math.Round(t.calculated_accuracy, 2)}%\n";

                        if (t.rank == "F")
                        {
                            info += $"{Math.Round(t.completion, 2)}% completed";
                        }
                        else
                        {
                            if (fc)
                            {
                                info += $"**{Math.Round(t.pp, 2)} PP**\n\n";
                            }
                            else
                            {
                                info += $"**{Math.Round(t.pp, 2)} PP** ♢ {Math.Round(t.nochokepp, 2)} PP if FC ({Math.Round(t.nochokeaccuracy, 2)}%)\n\n";
                            }
                        }

                        mapId = Convert.ToInt32(beatmap[0].beatmap_id);
                    }
                }

                if (total == 1)
                {
                    await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnUserRecent(user, firstBeatmap, firstRecent, info, mode, retryCount));
                }
                else
                {
                    await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnUserRecentList(user, firstBeatmap, info, mode));
                }

                if (mapId != 0)
                {
                    OsuModule.SetMapId(mapId);
                }
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
        }

        public async Task GetCompare(string osuId, bool findWithUsername)
        {
            try
            {
                var databaseProfile = await GetDatabaseEntityAsync(Module.Context.User.Id).ConfigureAwait(false);
                var mode = databaseProfile.OsuMode;

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseProfile.OsuId.ToString();
                    findWithUsername = false;
                }

                var user = GetJsonUser(osuId, findWithUsername, mode);
                var info = "";
                var mapId = Database.Queries.ChannelQueries.FindOrCreateChannel(Module.Context.Channel.Id).Result.OsuMapId;

                if (mapId == 0)
                {
                    throw new CakeException("`No beatmap found in channel`");
                }

                var scoreBuilder = new OsuScoreBuilder
                {
                    BeatmapId = mapId.ToString(),
                    Mode = mode.ToString(),
                    UserId = user.user_id.ToString()
                };

                var beatMapBuilder = new OsuBeatmapBuilder
                {
                    Mode = mode.ToString(),
                    ConvertedIncluded = "1",
                    BeatmapId = mapId
                };

                var beatmap = beatMapBuilder.Execute().FirstOrDefault();

                var score = scoreBuilder.Execute();

                if (score.Count == 0)
                {
                    throw new CakeException($"`No score(s) found found in {beatmap.complete_title}`");
                }

                foreach (var t in score)
                {
                    var modName = t.enabled_mods == "0" ? "No Mod" : OsuMods.Modnames(Convert.ToInt32(t.enabled_mods));

                    var dateTicks = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - t.date.Ticks);

                    var date = dateTicks.TotalDays > 60 ? TimeFormat.ToShortTimeSpan(dateTicks) : TimeFormat.ToLongTimeSpan(dateTicks);

                    info += $"***{modName}*** \n" +
                            $"⤷ **PP:** {Math.Round(t.pp, 0)} " +
                            $"**Rank:**{t.rank.LevelEmotes()} " +
                            $"**Accuracy:** {Math.Round(t.calculated_accuracy, 2)}% " +
                            $"**Combo:** {t.maxcombo}({beatmap.max_combo}) \n" +
                            $" {OsuEmoteCodes.Emote300} {t.count300} | {OsuEmoteCodes.Emote100} {t.count100} | {OsuEmoteCodes.Emote50} {t.count50} | {OsuEmoteCodes.EmoteX} {t.countmiss}\n " +
                            $" {date} ago\n\n";
                }

                await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnChannelCompare(user, beatmap, info, mode));
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
        }
    }
}
