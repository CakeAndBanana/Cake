using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Queries;
using Discord.Commands;
using Discord.WebSocket;

namespace CakeBot.Core
{
    public static class DeathMessage
    {
        public static async Task CreateDeathMessage(DiscordSocketClient _client, SocketMessage messageParams)
        {
            var name = Regex.Match(messageParams.Content, ".+?(?= died)").Value;
            var age = Convert.ToInt32(Regex.Match(messageParams.Content, @"([0-9]+)(?=\!)").Value);
            var commandContext = new CommandContext(_client, (SocketUserMessage)messageParams);

            await UserQueries.AddDeadChild(name, age, commandContext.Guild.Id);

            await messageParams.Channel.SendMessageAsync($"<:tombstone:545701627609481219> | {name} just died at age of {age}");
        }
    }
}
