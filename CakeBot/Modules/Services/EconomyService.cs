using System;
using System.Threading.Tasks;
using CakeBot.Helper;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using Discord;
using EmbedType = CakeBot.Helper.EmbedType;

namespace CakeBot.Modules.Services
{
    public class EconomyService : CustomBaseService
    {
        public async Task GetBalance()
        {
            try
            {
                var db = new CakeEntities();
                var money = EconomyQueries.FindMoney(Module.Context.User.Id, db);
                var builder = new CakeEmbedBuilder
                {
                    Title = "Balance",
                    ThumbnailUrl = Module.Context.User.GetAvatarUrl(),
                    Description = $"**{Module.Context.User.Username}**, you have 💶**{money.Result}** "
                };
                await SendEmbedAsync(builder);
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

        public async Task Daily()
        {
            try
            {
                var db = new CakeEntities();
                    await SendMessageAsync(
                        $"**{Module.Context.User.Username}**, " + await EconomyQueries.CheckDaily(Module.Context.User.Id, db));
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
        
        public async Task AddMoney(IGuildUser user, long amount)
        {
            try
            {
                var db = new CakeEntities();
                if (await EconomyQueries.AddMoney(user.Id, amount, db))
                {
                    await SendMessageAsync(
                        $"**{Module.Context.User.Username}**, you have added 💶 **{amount}** to your balance!");
                }
                throw new CakeException("cheater");
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

        public async Task RemoveMoney(IGuildUser user, long amount)
        {
            try
            {
                var db = new CakeEntities();
                if (await EconomyQueries.RemoveMoney(user.Id, amount, db))
                {
                    await SendMessageAsync(
                        $"**{Module.Context.User.Username}**, you have removed 💶 **{amount}** to your balance!");
                }
                throw new CakeException("cheater");
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

        public async Task GetRank()
        {
            try
            {
                await SendMessageAsync(
                    $"{Module.Context.User.Mention}, your current rank is **#{await UserQueries.GetUserRank(Module.Context.User.Id)}**");
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

        public async Task Leaderboard()
        {
            var embedInfo = new CakeEmbedBuilder()
                .WithTitle("CakeBot")
                .WithDescription("issue#52") as CakeEmbedBuilder;
            embedInfo.EmbedType = EmbedType.Info;

            await SendEmbedAsync(embedInfo);
        }

        public async Task Correctify()
        {
            try
            {
                var db = new CakeEntities();
                await SendMessageAsync(await UserQueries.CorrectLevels(db));
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
    }
}