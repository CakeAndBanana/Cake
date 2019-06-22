using Cake.Core.Logging;
using Cake.Database.Models;
using CakeBot.Helper.Modules.Osu.Builder;
using CakeBot.Helper.Modules.Osu.Model;
using LinqToDB.Common;
using System;
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
                var databaseUser = await Database.Queries.UserQueries.FindOrCreateUser(Module.Context.User.Id);
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
    }
}
