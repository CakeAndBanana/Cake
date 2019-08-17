﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

using Discord;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Handlers
{
    public static class MessageReactionHandler
    {

        private static HashSet<IMessageReactionHandle> messageReactionHandlers;

        static MessageReactionHandler()
        {
            messageReactionHandlers = new HashSet<IMessageReactionHandle>();
        }

        internal static Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            foreach (var handle in messageReactionHandlers)
            {
                handle.OnReactionAdded(arg1, arg2, arg3);
            }

            return Task.CompletedTask;
        }

        internal static Task OnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3) {

            foreach (var handle in messageReactionHandlers)
            {
                handle.OnReactionRemoved(arg1, arg2, arg3);
            }

            return Task.CompletedTask;
        }

        internal static Task OnReactionCleared(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2) {

            foreach (var handle in messageReactionHandlers)
            {
                handle.OnReactionCleared(arg1, arg2);
            }

            return Task.CompletedTask;
        }

        #region Util

        public static void AddMessageReactionHandle(IMessageReactionHandle messageReactionHandle)
        {
            if (messageReactionHandle == null)
            {
                throw new ArgumentNullException(nameof(messageReactionHandle));
            }

            messageReactionHandlers.Add(messageReactionHandle);
        }

        public static void RemoveMessageReactionHandle(IMessageReactionHandle messageReactionHandle)
        {
            messageReactionHandlers.Remove(messageReactionHandle);
        }

        #endregion
    }
}
