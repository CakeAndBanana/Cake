using System;
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
                var userBf = Bf4Data.GetPlayerInfo(platform, name);
                if (userBf == null)
                {
                    throw new CakeException("`User doesn't exist in Battlefield 4.`");
                }
                var embedBuilder = new CakeEmbedBuilder()
                    .WithAuthor(author =>
                    {
                        author
                            .WithName($"Battlefield 4 stats of {userBf.player.name}")
                            .WithUrl(userBf.player.blPlayer);
                    })
                    .WithThumbnailUrl(Bf4Helper.RankToUrl(userBf.stats.rank))
                    .WithDescription(
                    $"**Skill rating: **{ userBf.stats.skill }\n" +
                    $"**Kills: **{ userBf.stats.kills }\n" +
                    $"**Deaths: **{ userBf.stats.deaths }\n" +
                    $"**Headshots: **{ userBf.stats.headshots }\n" +
                    $"**KDR: **{ Math.Round(Bf4Helper.KDR(userBf.stats.kills, userBf.stats.deaths), 2) }\n" +
                    $"**Shots fired: **{ userBf.stats.shotsFired }\n" +
                    $"**Shots hit: **{ userBf.stats.shotsHit }\n" +
                    $"**Accuracy: **{ Math.Round(Bf4Helper.Accuracy(userBf.stats.shotsFired, userBf.stats.shotsHit), 2) }\n" +
                    $"**Longest Killstreak: **{ userBf.stats.killStreakBonus }\n" +
                    $"**Longest Headshot (Units): **{ userBf.stats.longestHeadshot }\n" +
                    $"**Wins: **{ userBf.stats.numWins }\n" +
                    $"**Losses: **{ userBf.stats.numLosses }\n" +
                    $"**W/L Ratio: **{ Bf4Helper.WLRatio(userBf.stats.numWins, userBf.stats.numLosses) }\n" +
                    $"**Time played: **{ Bf4Helper.TimePlayed(userBf.stats.timePlayed) }\n"
                    ) as CakeEmbedBuilder;
                await SendEmbedAsync(embedBuilder);
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        
    }
}
