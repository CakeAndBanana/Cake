using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Handlers
{
    public interface IMessageReactionHandle
    {
        /// <summary>
        /// Invoked when a reaction has been added to the message.
        /// </summary>
        Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3);

        /// <summary>
        /// Invoked when a reaction has been removed to the message.
        /// </summary>
        Task OnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3);

        /// <summary>
        /// Invoked when all the reactions has been cleared from the message.
        /// </summary>
        Task OnReactionCleared(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2);
    }
}
