using Discord.WebSocket;

namespace CakeBot.Core.Helper
{
    public class PrefixHelper
    {
        public static ulong GuildIdFinder(DiscordSocketClient client, ulong channelid)
        {
            ulong guildid = 0;

            foreach (var guild in client.Guilds)
            {
                foreach (var channel in guild.Channels)
                {
                    if (channel.Id == channelid)
                    {
                        guildid = guild.Id;
                    }
                }
            }

            return guildid;
        }
    }
}
