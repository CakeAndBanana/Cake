using System.Collections.Generic;
using System.Threading.Tasks;
using Cake.Core.Discord.Attributes;
using Cake.Core.Discord.Embed.Builder;
using Cake.Core.Discord.Handlers;
using Cake.Core.Discord.Services;
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
            List<CakeEmbedBuilder> helpPages = await _service.FetchAllCommandInfoAsPages(_commandService);

            HelpBook helpBook = new HelpBook(helpPages, Context.Message.Id);

            MessageReactionHandler.AddMessageReactionHandle(helpBook);

            // TODO: Command searching filter
        }
    }
}
