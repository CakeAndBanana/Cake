using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cake.Core.Discord.Embed.Builder;
using Cake.Core.Discord.Handlers;
using Discord;
using Discord.WebSocket;

namespace Cake.Core.Discord.Modules
{
    internal class HelpBook : IMessageReactionHandle
    {
        internal const string RightArrowEmojiDisplayName = "➡️";
        internal const string LeftArrowEmojiDisplayName = "⬅️";

        internal static readonly Emoji RightArrowEmoji = new Emoji(RightArrowEmojiDisplayName);
        internal static readonly Emoji LeftArrowEmoji = new Emoji(LeftArrowEmojiDisplayName);

        /// <summary>
        /// The ID of the message this help book is targeted towards.
        /// </summary>
        private ulong _targetMessageID;

        private List<CakeEmbedBuilder> _helpPages;

        private int _currentPageIndex;

        private CakeEmbedBuilder CurrentPage {
            get {
                return _helpPages[_currentPageIndex];
            }
        }

        internal HelpBook(List<CakeEmbedBuilder> helpPages, ulong targetMessageID) {
            _helpPages = helpPages;
            _targetMessageID = targetMessageID;
            _currentPageIndex = 0;
        }

        public Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (ReactionAdderIsBot()) {
                return Task.CompletedTask;
            }

            if (TryFlipPageByEmojiReaction()) {
                if (TryGetUserMessageFromCachedMessage(arg1, out IUserMessage userMessage))
                {
                    userMessage.RemoveReactionAsync(arg3.Emote, userMessage.Author);
                    ShowNewPageOnMessage(ref userMessage);
                }
                else {
                    // This message was probably deleted before we can do anything else.
                    MessageReactionHandler.RemoveMessageReactionHandle(this);
                }
            }

            return Task.CompletedTask;

            #region Local_Function

            Task ShowNewPageOnMessage(ref IUserMessage message) {
                message.ModifyAsync(msg => msg.Embed = CurrentPage.Build());

                return Task.CompletedTask;
            }
            bool ReactionAdderIsBot() {
                var reactionAdder = arg3.User;
                if (reactionAdder.IsSpecified) {
                    return reactionAdder.Value.IsBot;
                }

                return false;
            }

            bool TryFlipPageByEmojiReaction() {
                bool pageFlipped = false;

                if (arg3.Emote.Name.Equals(RightArrowEmojiDisplayName))
                {
                    FlipNextHelpPage();
                    pageFlipped = true;
                }
                else if (arg3.Emote.Name.Equals(LeftArrowEmojiDisplayName)) {
                    FlipPreviousHelpPage();
                    pageFlipped = true;
                }

                return pageFlipped;
            }

            #endregion
        }

        public Task OnReactionCleared(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (TryGetUserMessageFromCachedMessage(arg1, out IUserMessage userMessage))
            {
                // Add back the bot's reaction
                userMessage.AddReactionsAsync(new IEmote[]{ LeftArrowEmoji, RightArrowEmoji });
            }
            else {
                MessageReactionHandler.RemoveMessageReactionHandle(this);
            }

            return Task.CompletedTask;
        }

        public Task OnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            return Task.CompletedTask;
        }

        #region Util

        private bool TryGetUserMessageFromCachedMessage(Cacheable<IUserMessage, ulong> cachedMessage, out IUserMessage userMessage)
        {
            bool canGetMessage = cachedMessage.HasValue;

            if (canGetMessage)
            {
                userMessage = cachedMessage.Value;
            }
            else
            {
                userMessage = null;
            }

            return canGetMessage;
        }

        private void FlipNextHelpPage() {
            ++_currentPageIndex;
            ClampCurrentPageIndex();
        }

        private void FlipPreviousHelpPage() {
            --_currentPageIndex;
            ClampCurrentPageIndex();
        }

        private void ClampCurrentPageIndex() {
            if (_currentPageIndex >= _helpPages.Count)
            {
                _currentPageIndex = 0;
            }
            else if (_currentPageIndex <= 0) {
                _currentPageIndex = _helpPages.Count - 1;
            }
        }

        #endregion
    }
}
