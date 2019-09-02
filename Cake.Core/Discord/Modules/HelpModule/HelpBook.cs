using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Cake.Core.Discord.Embed.Builder;
using Cake.Core.Discord.Handlers;
using Discord;
using Discord.WebSocket;

namespace Cake.Core.Discord.Modules
{
    internal class HelpBook : IMessageReactionHandle
    {
        /// <summary>
        /// How long (in seconds) to wait for before this help-book becomes un-interactable.
        /// </summary>
        private const double inactivityDuration = 20;

        internal static readonly string RightArrowEmojiDisplayName;
        internal static readonly string LeftArrowEmojiDisplayName;
        internal static readonly Emoji RightArrowEmoji;
        internal static readonly Emoji LeftArrowEmoji;

        /// <summary>
        /// The ID of the message this help book is targeted towards.
        /// </summary>
        private ulong _targetMessageID;
         
        private List<CakeEmbedBuilder> _helpPages;

        private int _currentPageIndex;

        private Timer _inactivityTimer;

        private CakeEmbedBuilder CurrentPage
        {
            get
            {
                return _helpPages[_currentPageIndex];
            }
        }

        static HelpBook()
        {
            RightArrowEmojiDisplayName = ((char)11013).ToString();
            LeftArrowEmojiDisplayName = ((char)10145).ToString();

            RightArrowEmoji = new Emoji(RightArrowEmojiDisplayName);
            LeftArrowEmoji = new Emoji(LeftArrowEmojiDisplayName);
        }

        internal HelpBook(List<CakeEmbedBuilder> helpPages, ulong targetMessageID)
        {
            _helpPages = helpPages;
            _targetMessageID = targetMessageID;
            _currentPageIndex = 0;

            SetupInactivityTimer();

            #region Local_Function

            void SetupInactivityTimer() {
                _inactivityTimer = new Timer(inactivityDuration * 1000);
                _inactivityTimer.AutoReset = false;
                _inactivityTimer.Enabled = true;
                _inactivityTimer.Elapsed += OnInactivityTimerElapsed;

                _inactivityTimer.Start();
            }

            #endregion
        }

        private void OnInactivityTimerElapsed(object sender, ElapsedEventArgs e)
        {
            DisposeThisHelpBook();
        }

        public Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (ReactionAdderIsBot() || arg1.Id != _targetMessageID)
            {
                return Task.CompletedTask;
            }

            if (TryFlipPageByEmojiReaction())
            {
                IUserMessage userMessage = arg2.GetMessageAsync(arg1.Id).Result as IUserMessage;

                if (userMessage != null)
                {
                    userMessage.RemoveReactionAsync(arg3.Emote, arg3.User.Value);
                    ShowNewPageOnMessage(ref userMessage);
                    ResetTimer();
                }
                else
                {
                    // This message was probably deleted before we can do anything else.
                    DisposeThisHelpBook();
                }
            }

            return Task.CompletedTask;

            #region Local_Function

            Task ShowNewPageOnMessage(ref IUserMessage message)
            {
                message.ModifyAsync(msg => msg.Embed = CurrentPage.Build());

                return Task.CompletedTask;
            }
            bool ReactionAdderIsBot()
            {
                var reactionAdder = arg3.User;
                if (reactionAdder.IsSpecified)
                {
                    return reactionAdder.Value.IsBot;
                }

                return false;
            }

            bool TryFlipPageByEmojiReaction()
            {
                bool pageFlipped = false;

                if (arg3.Emote.Name.Equals(RightArrowEmojiDisplayName))
                {
                    FlipPreviousHelpPage();
                    pageFlipped = true;
                }
                else if (arg3.Emote.Name.Equals(LeftArrowEmojiDisplayName))
                {
                    FlipNextHelpPage();
                    pageFlipped = true;
                }

                return pageFlipped;
            }

            #endregion
        }

        public Task OnReactionCleared(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (arg1.Id != _targetMessageID) { return Task.CompletedTask; }

            IUserMessage userMessage = arg2.GetMessageAsync(arg1.Id).Result as IUserMessage;

            if (userMessage != null)
            {
                // Add back the bot's reaction
                userMessage.AddReactionsAsync(new Emoji[] { RightArrowEmoji, LeftArrowEmoji });
            }
            else
            {
                DisposeThisHelpBook();
            }

            return Task.CompletedTask;
        }

        public Task OnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            return Task.CompletedTask;
        }

        #region Util

        private void DisposeThisHelpBook() {
            MessageReactionHandler.RemoveMessageReactionHandle(this);
            DisposeTimer();
        }

        private void ResetTimer() {
            _inactivityTimer.Stop();
            _inactivityTimer.Start();
        }

        private void DisposeTimer() {
            _inactivityTimer.Stop();
            _inactivityTimer.Close();
            _inactivityTimer.Dispose();
        }

        private void FlipNextHelpPage()
        {
            ++_currentPageIndex;
            ClampCurrentPageIndex();
        }

        private void FlipPreviousHelpPage()
        {
            --_currentPageIndex;
            ClampCurrentPageIndex();
        }

        private void ClampCurrentPageIndex()
        {
            if (_currentPageIndex >= _helpPages.Count)
            {
                _currentPageIndex = 0;
            }
            else if (_currentPageIndex < 0)
            {
                _currentPageIndex = _helpPages.Count - 1;
            }
        }

        #endregion
    }
}
