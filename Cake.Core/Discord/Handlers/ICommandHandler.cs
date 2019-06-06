using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Cake.Core.Discord.Handlers
{
    internal interface ICommandHandler
    {
        Task InitializeAsync();

        Task HandleCommandEvent(SocketMessage message);
    }
}
