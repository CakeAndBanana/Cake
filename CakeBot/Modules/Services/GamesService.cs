﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CakeBot.Helper;
using CakeBot.Helper.Database.Model;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using CakeBot.Helper.Modules.BF4;

namespace CakeBot.Modules.Services
{
    public class GamesService : CustomBaseService
    {
        private readonly Random _random = new Random();

        public async Task CoinFlip()
        {
            try
            {
                var seed = _random.Next();
                var randomFlip = new Random(seed);
                var result = randomFlip.Next(2);

                var description = new StringBuilder();
                var resultString = result == 1 ? "Tails" : "Heads";
                description.Append($"**Result: ** {resultString}").Append("\n")
                    .Append($"**Seed: ** {seed}");

                var embed = new CakeEmbedBuilder(EmbedType.Info)
                    .WithTitle("Coin Flip")
                    .WithDescription(description.ToString()) as CakeEmbedBuilder;;

                await SendEmbedAsync(embed);
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

        public async Task SendDeaths(CakeBotEntities db)
        {
            try
            {
                string deathString = null;

                var deathchildren = await UserQueries.GetDeadChildren(db, Module.Context.Guild.Id);

                var list = (from u in deathchildren
                    orderby u.Id descending
                    select u).Take(15).ToList();

                var oldestChild = (from u in deathchildren
                    orderby u.Age descending
                    select u).FirstOrDefault();

                foreach (var child in list)
                {
                    deathString += $"<:tombstone:545701627609481219> {child.DateOfDeath:d/M/yyyy HH:mm} | {child.Name} died at the age of {child.Age}\n";
                }

                var builder = new CakeEmbedBuilder
                {
                    Title = "Graveyard",
                    Description = $"{deathString}\nOldest child was {oldestChild.Name}, he was {oldestChild.Age}\nMay they rest in peace."
                };

                builder.WithFooter($"{deathchildren.Count} children have died.");

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

        public async Task BattlefieldStats(string platform, string name, Discord.IUser user)
        {
            try
            {
                var UserBf = Bf4Stats.GetPlayerInfo(platform, name);
                var embedBuilder = new CakeEmbedBuilder();
                embedBuilder.WithAuthor(author =>
                {
                    author
                    .WithName($"Battlefield stats of {UserBf.player.name}");
                });
                // Dit werkt toch?, ik zie geen errors namelijk met live share xD/
                //Wat is de error? nu niks meer. Was gwn een typo. had UserBf4.
                // Test dit uit dat het werkt.
                //Uhm pim. bij BF4.cs
            }
            catch(CakeException e)
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
