using Cake.Core.Discord.Embed.Builder;
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
                    await SendMessageAsync(
                        "`No osu account has been linked to this discord account, use osu set (username / user id).`");
                }
            }
            catch (Exception e)
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
                throw new Exception("``User with given username or id is not found on osu!``");
            }

            return user;
        }

        public async Task SetAccount(string username)
        {
            var databaseprofile = await Database.Queries.UserQueries.FindOrCreateUser(Module.Context.User.Id);
            var user = GetJsonUser(username, true);

            databaseprofile.OsuId = user.user_id;
            await Database.Queries.UserQueries.Update(databaseprofile);

            await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnSetAccountEmbed(user));
        }

        public async Task GetUserProfile(string osuId, bool findWithUsername)
        {
            try
            {
                var databaseUser = await GetDatabaseEntityAsync(Module.Context.User.Id);
                var mode = databaseUser.OsuMode;

                if (osuId.IsNullOrEmpty())
                {
                    osuId = databaseUser.OsuId.ToString();
                    findWithUsername = false;
                }
                var user = GetJsonUser(osuId, findWithUsername, mode);

                await SendEmbedAsync(Embeds.OsuModuleEmbeds.ReturnUserProfile(user, mode));
            }
            catch (Exception e)
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
                var databaseProfile = await GetDatabaseEntityAsync(Module.Context.User.Id);
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

                    var dateTicks = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - item.date.Ticks);

                    var date = dateTicks.TotalDays > 30 ? TimeFormat.ToShortTimeSpan(dateTicks) : TimeFormat.ToLongTimeSpan(dateTicks);

                    var starRating = Math.Abs(item.starrating) <= 0 ? result[0].difficultyrating : item.starrating;

                    fields.Add(new Tuple<string, string>($"#{item.play_number}: {result[0].complete_title} {OsuMods.Modnames(Convert.ToInt32(item.enabled_mods))} {Math.Round(starRating, 2)}★",
                                  $@"⤷ **PP:** {Math.Round(item.pp, 0)} " +
                                  $"**Rank:** {item.rank.LevelEmotes()} " +
                                  $"**Combo:** {item.maxcombo}({result[0].max_combo}) \n" +
                                  $" {OsuUtil.Emote300} {item.count300} ♢ {OsuUtil.Emote100} {item.count100} ♢ {OsuUtil.Emote50} {item.count50} ♢ {OsuUtil.EmoteX} {item.countmiss} ♢ {Math.Round(item.calculated_accuracy, 2)}%\n" +
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
    }
}
