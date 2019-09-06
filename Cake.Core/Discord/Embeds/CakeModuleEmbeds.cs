using Cake.Core.Discord.Embed.Builder;
using Discord;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cake.Core.Discord.Embed
{
    public class CakeModuleEmbeds
    {
        protected CakeModuleEmbeds()
        {
        }
        public static CakeEmbedBuilder ReturnStatusEmbed(IGuild guild)
        {
            CakeEmbedBuilder statusEmbedBuilder = new CakeEmbedBuilder();
            statusEmbedBuilder.WithAuthor($"{Main.GetClient().CurrentUser.Username}");
            statusEmbedBuilder.WithTitle($"Status of {Main.GetClient().CurrentUser.Username}");
            statusEmbedBuilder.WithDescription($"Version: ``{Json.CakeJson.GetConfig().Version}``");

            statusEmbedBuilder.WithFields(GetStatusFields());

            statusEmbedBuilder.WithFooter(GetUptimeAsString());

            return statusEmbedBuilder;

            #region Local_Function
            string GetUptimeAsString()
            {
                TimeSpan upTimeSpan = DateTime.UtcNow.Subtract(Process.GetCurrentProcess().StartTime.ToUniversalTime());

                StringBuilder message = new StringBuilder(string.Empty);

                int days = upTimeSpan.Days;
                int months = GetEstimatedMonthFromSubtractingDays(ref days);
                int years = GetYearsFromSubtractingMonths(ref months);

                // TODO: Possible refactor?
                AppendTimeToStringIfPositive(ref message, "Year", years);
                AppendTimeToStringIfPositive(ref message, "Month", months);
                AppendTimeToStringIfPositive(ref message, "Day", days);
                AppendTimeToStringIfPositive(ref message, "Hour", upTimeSpan.Hours);
                AppendTimeToStringIfPositive(ref message, "Minute", upTimeSpan.Minutes);

                var upTimeString = message.ToString();
                RemoveLastCommaIfExists(ref upTimeString);

                return upTimeString;
            }

            void RemoveLastCommaIfExists(ref string str)
            {
                int lastCommaIndex = str.LastIndexOf(",");
                if (lastCommaIndex != -1)
                {
                    str = str.Substring(0, lastCommaIndex);
                }
            }

            void AppendTimeToStringIfPositive(ref StringBuilder str, string timeText, int time)
            {
                if (time > 0)
                {
                    str.Append($"{time} {timeText}{GivePluralIfNeeded(time)}, ");
                }
            }

            string GivePluralIfNeeded(int test)
            {
                if (test > 1)
                {
                    return "s";
                }
                return string.Empty;
            }

            int GetEstimatedMonthFromSubtractingDays(ref int days)
            {
                int month = 0;
                while (days >= 30)
                {
                    ++month;
                    days -= 30;
                }
                return month;
            }

            int GetYearsFromSubtractingMonths(ref int months)
            {
                int years = 0;
                while (months >= 12)
                {
                    ++years;
                    months -= 12;
                }
                return years;
            }

            EmbedFieldBuilder[] GetStatusFields()
            {
                List<EmbedFieldBuilder> statusFields = new List<EmbedFieldBuilder>();
                statusFields.Add(
                    new EmbedFieldBuilder().WithName("Discord Latency")
                        .WithValue($"{Main.GetClient().Latency}ms")
                    );

                statusFields.Add(
                    new EmbedFieldBuilder().WithName("Guild Shards / Total Shards")
                       .WithValue($"{Main.GetClient().GetShardIdFor(guild) + 1} / {Main.GetClient().Shards.Count}")
                    );

                return statusFields.ToArray();
            }

            #endregion
        }
    }
}
