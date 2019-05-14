using System.Threading.Tasks;
using CakeBot.Modules.Services;
using Discord.Commands;

namespace CakeBot.Modules.Modules
{
    [Name("Cake")]
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

        [Command("help")]
        [Summary(">help")]
        [Remarks("Returns the commands of the bot")]
        [Alias("cmds", "commands")]
        public async Task CommandHelp([Remainder] string command = "")
        {
            if (command == "")
            {
                await _service.HelpAll(_commandService);
            }
            else
            {
                await _service.HelpCommand(_commandService, command);
            }
        }
    }
}