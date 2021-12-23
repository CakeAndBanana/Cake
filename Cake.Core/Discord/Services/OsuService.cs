using Cake.Core.Discord.Modules;
using Cake.Core.Exceptions;
using Cake.Core.Extensions.Osu;
using Cake.Core.Extensions;
using Cake.Database.Models;
using Cake.Json.CakeBuilders.Osu;
using Cake.Json.CakeModels.Osu;
using LinqToDB.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core.Logging;

namespace Cake.Core.Discord.Services
{
    public class OsuService : CustomBaseService
    {
        private readonly Logger _logger = Logger.Get() as Logger;

        private async Task<CakeUser> GetDatabaseEntityAsync(ulong discordId)
        {
            var databaseProfile = await Database.Queries.UserQueries.FindOrCreateUser(discordId);

            try
            {
                if (databaseProfile.OsuId == 0)
                {
                    throw new CakeException("`No osu account has been linked to this discord account, use osu set (username / user id).`");
                }
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
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
                return null;
            }
            return user;
        }

        public async Task SetAccount(string username)
        {
            try
            {
                var databaseprofile = await Database.Queries.UserQueries.FindOrCreateUser(Module.Context.User.Id).ConfigureAwait(false);
                var user = GetJsonUser(username, true);
                
                if (user !=null)
                {
                    databaseprofile.OsuId = user.user_id;
                    await Database.Queries.UserQueries.Update(databaseprofile);

                    await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnSetAccountEmbed(user));
                }
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
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

        public async Task GetUserProfile(string osuId, bool findWithUsername, bool dUser = false, ulong dUserId = 0)
        {
            try
            {
                CakeUser databaseUser = !dUser
                    ? await GetDatabaseEntityAsync(Module.Context.User.Id)
                    : await GetDatabaseEntityAsync(dUserId);

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseUser.OsuId.ToString();
                    findWithUsername = false;
                }
                var user = GetJsonUser(osuId, findWithUsername, databaseUser.OsuMode);

                await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnUserProfile(user, databaseUser.OsuMode));
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
        }

        public async Task GetUserBest(string osuId, bool findWithUsername, bool recent, int? play, bool dUser = false, ulong dUserId = 0)
        {
            try
            {
                var fields = new List<Tuple<string, string>>();
                CakeUser databaseUser = !dUser
                    ? await GetDatabaseEntityAsync(Module.Context.User.Id)
                    : await GetDatabaseEntityAsync(dUserId);

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseUser.OsuId.ToString();
                    findWithUsername = false;
                }

                var user = GetJsonUser(osuId, findWithUsername, databaseUser.OsuMode);

                var bestBuilder = new OsuUserBestBuilder
                {
                    Mode = databaseUser.OsuMode,
                    Limit = (recent || play != null ? 100 : 5).ToString(),
                    UserId = user.user_id,
                    Recent = recent,
                    PlayNumber = play
                };

                var best = OsuTimeConverter.ConvertScorableDate(user.country, bestBuilder.Execute());
                
                foreach (var item in best)
                {
                    if (play != null)
                    {
                        item.play_number = (int)play;
                    }

                    var dateTicks = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - item.date.UtcTicks);
                    var timeFormat = new TimeFormat(dateTicks);

                    var date = dateTicks.TotalDays > 30 ? timeFormat.ToShortString() : timeFormat.ToLongString();

                    var starRating = Math.Abs(item.starrating) <= 0 ? item.Beatmap.difficultyrating : item.starrating;

                    fields.Add(new Tuple<string, string>($"#{item.play_number}: {item.Beatmap.complete_title} {OsuMods.Modnames(Convert.ToInt32(item.enabled_mods))} {Math.Round(starRating, 2)}★",
                                  $@"⤷ **PP:** {Math.Round(item.pp, 0)} " +
                                  $"**Rank:** {item.rank.LevelEmotes()} " +
                                  $"**Combo:** {item.maxcombo}({item.Beatmap.max_combo}) \n" +
                                  $" {OsuEmoteCodes.Emote300} {item.count300} ♢ {OsuEmoteCodes.Emote100} {item.count100} ♢ {OsuEmoteCodes.Emote50} {item.count50} ♢ {OsuEmoteCodes.EmoteX} {item.countmiss} ♢ {Math.Round(item.calculated_accuracy, 2)}%\n" +
                                  $" **Downloads:** [Beatmap]({item.Beatmap.beatmap_url})" +
                                  $"([no vid]({item.Beatmap.beatmap_url + "n"})) " +
                                  $"[Bloodcat]({item.Beatmap.bloodcat})\n" +
                                  $" {date} ago\n"));
                }

                await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnUserBest(user, $"https://b.ppy.sh/thumb/{best.First().Beatmap.beatmapset_id}l.jpg", fields, databaseUser.OsuMode));
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e);
            }
        }

        public async Task GetUserRecent(string osuId, bool findWithUsername, bool dUser = false, ulong dUserId = 0)
        {
            try
            {
                CakeUser databaseUser = !dUser
                    ? await GetDatabaseEntityAsync(Module.Context.User.Id)
                    : await GetDatabaseEntityAsync(dUserId);
                    
                var info = "";

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseUser.OsuId.ToString();
                    findWithUsername = false;
                }

                var user = GetJsonUser(osuId, findWithUsername, databaseUser.OsuMode);

                var recentBuilder = new OsuUserRecentBuilder
                {
                    Mode = databaseUser.OsuMode,
                    Limit = "1",
                    UserId = user.user_id
                };
                var recent = OsuTimeConverter.ConvertScorableDate(user.country, recentBuilder.Execute());

                var retryCount = 0;

                if (recent.Count == 0)
                {
                    throw new CakeException($"`No recent play(s) has been found for {user.username}`");
                }

                for (var i = 0; i < recent.Count; i++)
                {
                    var t = recent[i];

                    retryCount = OsuCheckRetries.Tries(databaseUser.OsuMode, t.user_id, t.Beatmap.beatmap_id);
                    info = $"**{t.rounded_score} ♢ " +
                                  $"{t.rank.LevelEmotes()} ♢ {t.maxcombo}x*({ t.Beatmap.max_combo}x)*** {OsuMods.Modnames(Convert.ToInt32(t.enabled_mods))} \n " +
                                  $"{OsuEmoteCodes.Emote300} {t.count300} ♢ {OsuEmoteCodes.Emote100} {t.count100} ♢ {OsuEmoteCodes.Emote50} {t.count50} ♢ {OsuEmoteCodes.EmoteX} {t.countmiss} ♢ {Math.Round(t.calculated_accuracy, 2)}%\n";

                    if (t.rank == "F")
                    {
                        info += $"{Math.Round(t.completion, 2)}% completed ♢ if completed **{Math.Round(t.pp, 2)} PP**\n";
                    }
                    else
                    {
                        if (!t.choked)
                        {
                            info += $"**{Math.Round(t.pp, 2)} PP**\n\n";
                        }
                        else
                        {
                            info += $"**{Math.Round(t.pp, 2)} PP** ♢ {Math.Round(t.nochokepp, 2)} PP if FC ({Math.Round(t.nochokeaccuracy, 2)}%)\n\n";
                        }
                    }
                }
                
                await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnUserRecent(user, recent[0].Beatmap, recent[0], info, databaseUser.OsuMode, retryCount));
                
                if (recent[0].Beatmap.beatmap_id != 0)
                {
                    OsuModule.SetMapId(recent[0].Beatmap.beatmap_id);
                }
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e);
            }
        }

        public async Task GetCompare(string osuId, bool findWithUsername, bool dUser = false, ulong dUserId = 0)
        {
            try
            {
                CakeUser databaseUser = null;
                if (!dUser)
                {
                    databaseUser = await GetDatabaseEntityAsync(Module.Context.User.Id).ConfigureAwait(false);
                }
                else
                {
                    databaseUser = await GetDatabaseEntityAsync(dUserId).ConfigureAwait(false);
                }
                var mode = databaseUser.OsuMode;

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseUser.OsuId.ToString();
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
                    BeatmapId = mapId,
                    Mode = mode,
                    UserId = user.user_id
                };

                var score = scoreBuilder.Execute();

                if (!score.Any())
                {
                    throw new CakeException($"`No score(s) found in {mapId}`");
                }

                foreach (var t in score)
                {
                    var modName = t.enabled_mods == "0" ? "No Mod" : OsuMods.Modnames(Convert.ToInt32(t.enabled_mods));

                    var dateTicks = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - t.date.UtcTicks);
                    var timeFormat = new TimeFormat(dateTicks);

                    var date = dateTicks.TotalDays > 30 ? timeFormat.ToShortString() : timeFormat.ToLongString();

                    info += $"***{modName}*** \n";
                    if (t.pp != null)
                    {
                        info += $"⤷ **PP:** {Math.Round((double)t.pp, 0)} " +
                            $"**Rank:**{t.rank.LevelEmotes()} ";
                    }
                    else
                    {
                        info += $"⤷ **Rank:**{t.rank.LevelEmotes()} ";
                    }

                    info += $"**Accuracy:** {Math.Round(t.calculated_accuracy, 2)}% " +
                            $"**Combo:** {t.maxcombo}({t.Beatmap.max_combo}) \n" +
                            $" {OsuEmoteCodes.Emote300} {t.count300} ♢ {OsuEmoteCodes.Emote100} {t.count100} ♢ {OsuEmoteCodes.Emote50} {t.count50} ♢ {OsuEmoteCodes.EmoteX} {t.countmiss}\n " +
                            $" {date} ago\n\n";
                }

                await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnChannelCompare(user, score[0].Beatmap, info, mode));
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                await SendMessageAsync(e.ToString());
                _logger.LogError(e);
            }
        }
    }
}
