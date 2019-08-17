﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Cake.Core.Discord.Attributes;
using Cake.Core.Discord.Embed.Builder;
using Cake.Core.Discord.Handlers;
using Cake.Core.Discord.Services;
using Discord;
using Discord.Commands;

namespace Cake.Core.Discord.Modules
{
    [Hide]
    
    public class HelpModule : CustomBaseModule
    {
        private readonly CommandService _commandService;
        private readonly HelpService _service;

        public HelpModule(HelpService service, CommandService commandService)
        {
            _commandService = commandService;
            _service = service;
            _service.SetBaseModule(this);
        }

        [Hide]
        [Command("help")]
        [Alias("cmds", "commands")]
        public async Task CommandHelp([Remainder] string command = "")
        {
            IUserMessage newMessage = await Context.Channel.SendMessageAsync("Fetching Commands...");

            List<CakeEmbedBuilder> helpPages = await _service.FetchAllCommandInfoAsPages(_commandService);

            await newMessage.ModifyAsync(msg => { msg.Embed = helpPages[0].Build(); msg.Content = string.Empty; });

            await newMessage.AddReactionsAsync(new Emoji[] { HelpBook.RightArrowEmoji, HelpBook.LeftArrowEmoji });

            HelpBook helpBook = new HelpBook(helpPages, newMessage.Id);

            MessageReactionHandler.AddMessageReactionHandle(helpBook);

            // TODO: Command searching filter
        }
    }
}
