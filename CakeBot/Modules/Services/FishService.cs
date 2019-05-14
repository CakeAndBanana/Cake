using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CakeBot.Helper;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using CakeBot.Helper.Modules.Fishy;

namespace CakeBot.Modules.Services
{
    public class FishService : CustomBaseService
    {
        public async Task Fish(Random r, CakeBotEntities db)
        {
            try
            {
                if (!await FishQueries.CheckEligibility(Module.Context.User.Id)) {
                    await Module.Context.Channel.SendMessageAsync($"You need to wait {await FishQueries.GetTimeRemaining(Module.Context.User.Id)} seconds.");
                    return;
                }
                if (!await UserQueries.CheckUser(Module.Context.User.Id)) await Module.Context.Channel.SendMessageAsync("We added you to the Database, you can now use any command.");
                if (!UserQueries.CheckMoney(Module.Context.User.Id, db))
                {
                    await Module.Context.Channel.SendMessageAsync($"**{Module.Context.User.Username}**, you have Insufficient funds!");
                    return;
                }
                Catch usercatch = new Catch(Module.Context.User.Id, r, db);
                await Module.Context.Channel.SendMessageAsync(usercatch.ToString(Module.Context.User));
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await Module.Context.Channel.SendMessageAsync("", embed: embedError.Build());
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public async Task AddFish(string name, long worth, int rarity, string emoji, [global::Discord.Commands.Remainder] string message)
        {
            if (!Regex.IsMatch(emoji, "<[a-z]?:([a-z]+)(:[0-9]+)>"))
            {
                await Module.Context.Channel.SendMessageAsync("Emoji isnt correct format, please use a guild emote");
                return;
            }
            FishType fishy = new FishType()
            {
                FishEmoji = emoji,
                FishName = name,
                FishActive = true,
                FIshWorth = worth,
                FishMessage = message,
                FishRarity = rarity,
            };
            if (await FishQueries.AddFish(fishy))
            {
                await Module.Context.Channel.SendMessageAsync("Fish added!");
            }
            else await Module.Context.Channel.SendMessageAsync("Something went haywire");
        }

        public async Task getFishInv(CakeBotEntities db)
        {
            var fishes = db.UserFishInventories.Where(u => u.UserId == (long)Module.Context.User.Id);

            int trashcount = 0;
            int comcount = 0;
            int exocount = 0;
            int legcount = 0;

            // Return an EmbedBuilder that contains the inventory of the user
            StringBuilder commons = new StringBuilder();
            commons.AppendLine("**Commons:**");
            StringBuilder exotics = new StringBuilder();
            exotics.AppendLine("**Exotics:**");
            StringBuilder trash = new StringBuilder();
            trash.AppendLine("**Trash:**");
            StringBuilder legendary = new StringBuilder();
            legendary.AppendLine("**Legendary:**");
            var embed = new CakeEmbedBuilder(EmbedType.Info)
            {
                ThumbnailUrl = Module.Context.User.GetAvatarUrl(),
            };
            foreach (var fish in fishes)
            {
                if (!(fish.Amount > 0)) continue;
                switch (fish.FishType.FishRarity)
                {
                    case 0:
                        if ((trashcount % 4) == 0 && trashcount > 0) trash.Append($"\n{fish.FishType.FishEmoji}`[x{fish.Amount}]`");
                        else trash.Append($"{fish.FishType.FishEmoji}`[x{fish.Amount}]`");
                        trashcount++;
                        break;
                    case 1:
                        if ((comcount % 4) == 0 && comcount > 0) commons.Append($"\n{fish.FishType.FishEmoji}`[x{fish.Amount}]`");
                        else commons.Append($"{fish.FishType.FishEmoji}`[x{fish.Amount}]`");
                        comcount++;
                        break;
                    case 2:
                        if ((exocount % 4) == 0 && exocount > 0) exotics.Append($"\n{fish.FishType.FishEmoji}`[x{fish.Amount}]`");
                        else exotics.Append($"{fish.FishType.FishEmoji}`[x{fish.Amount}]`");
                        exocount++;
                        break;
                    case 3:
                        if ((legcount % 4) == 0 && legcount > 0) legendary.Append($"\n{fish.FishType.FishEmoji}`[x{fish.Amount}]`");
                        else legendary.Append($"{fish.FishType.FishEmoji}`[x{fish.Amount}]`");
                        legcount++;
                        break;
                }
            }

            embed.WithAuthor(author =>
            {
                author
                    .WithName($"{Module.Context.User.Username}'s Inventory")
                    .WithIconUrl("https://st2.depositphotos.com/3557671/11719/v/950/depositphotos_117192346-stock-illustration-fish-icon-flat-style-one.jpg");
            });
            embed.Description = $"{trash}\n{commons}\n{exotics}\n{legendary}";

            await Module.Context.Channel.SendMessageAsync("", embed: embed.Build());
        }

        public async Task SellFishes(string input, CakeBotEntities db)
        {
            try
            {
                if (int.TryParse(input, out int FishId))
                {
                    var result = await FishQueries.SellFish(Module.Context.User.Id, FishId, db);
                    await Module.Context.Channel.SendMessageAsync($"**{Module.Context.User.Username}**, {result}");
                }
                else
                {
                    var result = await FishQueries.SellFishByGroup(Module.Context.User.Id, input, db);
                    await Module.Context.Channel.SendMessageAsync($"**{Module.Context.User.Username}**, {result}");
                }
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await Module.Context.Channel.SendMessageAsync("", embed: embedError.Build());
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public async Task GetFishStats(CakeBotEntities db)
        {
            var fishes = db.UserFishInventories.Where(u => u.UserId == (long)Module.Context.User.Id);

            long castCount = 0;
            int trashcount = 0;
            int comcount = 0;
            int exocount = 0;
            int legcount = 0;

            // Return an EmbedBuilder that contains the inventory of the user
            StringBuilder commons = new StringBuilder();
            commons.AppendLine("**Commons:**");
            StringBuilder exotics = new StringBuilder();
            exotics.AppendLine("**Exotics:**");
            StringBuilder trash = new StringBuilder();
            trash.AppendLine("**Trash:**");
            StringBuilder legendary = new StringBuilder();
            legendary.AppendLine("**Legendary:**");
            var embed = new CakeEmbedBuilder(EmbedType.Info)
            {
                ThumbnailUrl = Module.Context.User.GetAvatarUrl(),
            };
            foreach (var fish in fishes)
            {
                if ((fish.Amount + fish.AmountSold) < 1) continue;
                switch (fish.FishType.FishRarity)
                {
                    case 0:
                        if ((trashcount % 4) == 0 && trashcount > 0) trash.Append($"\n{fish.FishType.FishEmoji}`[x{fish.Amount + fish.AmountSold}]`");
                        else trash.Append($"{fish.FishType.FishEmoji}`[x{fish.Amount + fish.AmountSold}]`");
                        trashcount++;
                        break;
                    case 1:
                        if ((comcount % 4) == 0 && comcount > 0) commons.Append($"\n{fish.FishType.FishEmoji}`[x{fish.Amount + fish.AmountSold}]`");
                        else commons.Append($"{fish.FishType.FishEmoji}`[x{fish.Amount + fish.AmountSold}]`");
                        comcount++;
                        break;
                    case 2:
                        if ((exocount % 4) == 0 && exocount > 0) exotics.Append($"\n{fish.FishType.FishEmoji}`[x{fish.Amount + fish.AmountSold}]`");
                        else exotics.Append($"{fish.FishType.FishEmoji}`[x{fish.Amount + fish.AmountSold}]`");
                        exocount++;
                        break;
                    case 3:
                        if ((legcount % 4) == 0 && legcount > 0) legendary.Append($"\n{fish.FishType.FishEmoji}`[x{fish.Amount + fish.AmountSold}]`");
                        else legendary.Append($"{fish.FishType.FishEmoji}`[x{fish.Amount + fish.AmountSold}]`");
                        legcount++;
                        break;
                }

            }
            castCount = await FishQueries.GetTotalCasts(Module.Context.User.Id);

            embed.WithAuthor(author =>
            {
                author
                    .WithName($"{Module.Context.User.Username}'s Statistics")
                    .WithIconUrl("https://st2.depositphotos.com/3557671/11719/v/950/depositphotos_117192346-stock-illustration-fish-icon-flat-style-one.jpg");
            });

            embed.Description = $"{trash}\n{commons}\n{exotics}\n{legendary}\n\nTotal Casts: **{castCount}**";

            await Module.Context.Channel.SendMessageAsync("", embed: embed.Build());
        }

        public async Task GetFishes(CakeBotEntities db)
        {
            var embed = new CakeEmbedBuilder(EmbedType.Success);
            var fishes = db.FishTypes.ToList();
            for (int i = 0; i < fishes.Count; i++)
            {
                if ((i % 3) == 0) embed.Description += $"\n[{fishes[i].FishId}]{fishes[i].FishEmoji} | ";
                else embed.Description += $"[{fishes[i].FishId}]{fishes[i].FishEmoji} | ";

            }
            embed.Title = $"**List of fishes:**";
            await Module.Context.Channel.SendMessageAsync("", embed: embed.Build());
        }
    }
}