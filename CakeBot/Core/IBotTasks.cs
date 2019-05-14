using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace CakeBot.Core
{
    public interface IBotTasks
    {
        Task HandleMessage(SocketMessage messageParams);
        Task SetupCommands();
        Task Ready();
        Task UserJoined(SocketGuildUser joinedUser);
        Task UserLeft(SocketGuildUser leftUser);
        Task Connected();
        Task Disconnected(Exception ex);
        Task Log(LogMessage message);
    }
}
