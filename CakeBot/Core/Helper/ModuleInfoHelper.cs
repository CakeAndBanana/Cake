using System.Collections.Generic;
using System.Linq;
using CakeBot.Helper.Logging;
using Discord;
using Discord.Commands;
using Tweetinvi.Core.Extensions;

namespace CakeBot.Core.Helper
{
    public class ModuleInfoHelper
    {
        private static ModuleInfoHelper _instance;

        private static List<ModuleInfo> _modules;
        private static List<ModuleInfo> _normalModules;
        private static List<ModuleInfo> _groupModules;
        private static Dictionary<ModuleInfo, IReadOnlyList<CommandInfo>> _commandsDictionary;

        public static ModuleInfoHelper Get()
        {
            return _instance ?? (_instance = new ModuleInfoHelper());
        }

        public void Init(IEnumerable<ModuleInfo> modules)
        {
            _modules = modules.ToList();
            _normalModules = new List<ModuleInfo>();
            _groupModules = new List<ModuleInfo>();
            _commandsDictionary = new Dictionary<ModuleInfo, IReadOnlyList<CommandInfo>>();

            foreach (var moduleInfo in _modules)
            {
                Logger.LogDebug("~ Found Module " + moduleInfo.Name + " ~");

                _commandsDictionary.Add(moduleInfo, moduleInfo.Commands);
                if (moduleInfo.Aliases.IsEmpty())
                {
                    _normalModules.Add(moduleInfo);
                }
                else
                {
                    _groupModules.Add(moduleInfo);
                }
            }
        }

        private CommandInfo GetNormalCommand(string commandName)
        {
            foreach (var moduleInfo in _normalModules)
            {
                foreach (var moduleInfoCommand in moduleInfo.Commands)
                {
                    if (moduleInfoCommand.Name.Equals(commandName))
                    {
                        return moduleInfoCommand;
                    }
                }
            }

            return null;
        }

        private CommandInfo GetGroupCommand(ModuleInfo moduleInfo, string commandName)
        {
            return moduleInfo.Commands
                .FirstOrDefault(command => command.Name.Contains(commandName));
        }

        public CommandInfo GetCommand(IUserMessage contextMessage)
        {
            var message = contextMessage.Content.Split(null);
            foreach (var moduleInfo in _modules)
            {
                if (moduleInfo.Aliases.Contains(message[0].TrimStart(Config.BotPrefix.ToCharArray())))
                {
                    return GetGroupCommand(moduleInfo, message[1]);
                }
            }

            return GetNormalCommand(message[0].TrimStart(Config.BotPrefix.ToCharArray()));
        }
    }
}
