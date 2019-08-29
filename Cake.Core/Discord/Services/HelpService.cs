using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core.Discord.Embed.Builder;
using Discord;
using Discord.Commands;

namespace Cake.Core.Discord.Services
{
    public class HelpService : CustomBaseService
    {
        public Task<List<CakeEmbedBuilder>> FetchAllCommandInfoAsPages(CommandService service, string commandSearchFilter = "")
        {
            List<CakeEmbedBuilder> helpPage = new List<CakeEmbedBuilder>();

            int pageNumber = 1;

            foreach (ModuleInfo module in service.Modules)
            {
                if (ModuleHasHideAttribute(module) || module.Commands.Count == 0) { continue; }

                CakeEmbedBuilder newEmbedPage = new CakeEmbedBuilder();

                PopulateEmbedWithModuleInfo(module, ref newEmbedPage);
                PopulateEmbedFieldsWithModuleCommands(module, ref newEmbedPage, commandSearchFilter);

                if (newEmbedPage.Fields.Count > 0)
                {
                    newEmbedPage.WithFooter(pageNumber.ToString());
                    helpPage.Add(newEmbedPage);
                    ++pageNumber;
                }
            }

            return Task.FromResult(helpPage);
        }

        private void PopulateEmbedFieldsWithModuleCommands(ModuleInfo moduleInfo, ref CakeEmbedBuilder cakeEmbedBuilder, string commandSearchFilter = "")
        {

            foreach (CommandInfo command in moduleInfo.Commands)
            {
                if (!CanAddCommandToEmbedField(command)) { continue; }

                EmbedFieldBuilder commandField = GetEmbedFieldWithCommandInfo(command);
                cakeEmbedBuilder.AddField(commandField);
            }

            #region Local_Function

            bool CanAddCommandToEmbedField(CommandInfo command)
            {
                if (CommandHasHideAttribute(command)) { return false; }

                if (!string.IsNullOrWhiteSpace(commandSearchFilter))
                {
                    if (!CommandContainsSearchFilter(command))
                    {
                        return false;
                    }
                }

                return true;
            }

            bool CommandContainsSearchFilter(CommandInfo command)
            {
                if (!string.IsNullOrWhiteSpace(command.Name))
                {
                    if (command.Name.Contains(commandSearchFilter))
                    {
                        return true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(command.Remarks))
                {
                    if (command.Remarks.Contains(commandSearchFilter))
                    {
                        return true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(command.Summary))
                {
                    if (command.Summary.Contains(commandSearchFilter))
                    {
                        return true;
                    }
                }

                return false;
            }

            EmbedFieldBuilder GetEmbedFieldWithCommandInfo(CommandInfo commandInfo)
            {
                EmbedFieldBuilder commandField = new EmbedFieldBuilder();
                commandField.WithIsInline(true);
                commandField.WithName(commandInfo.Name);
                commandField.WithValue(GetCommandDescriptionFromCommandInfo(commandInfo));
                return commandField;
            }

            string GetCommandDescriptionFromCommandInfo(CommandInfo commandInfo)
            {
                return $"`{commandInfo.Summary}` { System.Environment.NewLine + commandInfo.Remarks}";
            }

            #endregion
        }

        private void PopulateEmbedWithModuleInfo(ModuleInfo moduleInfo, ref CakeEmbedBuilder cakeEmbedBuilder)
        {
            cakeEmbedBuilder.WithTitle(GetModuleName());
            cakeEmbedBuilder.WithDescription(GetModuleDescription());

            #region Local_Function

            string GetModuleDescription()
            {
                string moduleDescription = moduleInfo.Summary;

                if (string.IsNullOrWhiteSpace(moduleDescription))
                {
                    moduleDescription = moduleInfo.Remarks;
                }

                return moduleDescription;
            }

            string GetModuleName()
            {
                string moduleName = moduleInfo.Name;
                // TODO: Possible refactor?

                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    moduleName = moduleInfo.Summary;
                }

                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    moduleName = moduleInfo.Remarks;
                }

                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    moduleName = moduleInfo.Group;
                }

                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    // No 'name', 'group', 'summary' or 'remark' attribute attached to it;
                    moduleName = moduleInfo.GetType().Name;
                }

                return moduleName;
            }

            #endregion
        }

        private bool ModuleHasHideAttribute(ModuleInfo module)
        {
            return module.Attributes.Any(x => x.TypeId.ToString() == "Cake.Core.Discord.Attributes.HideAttribute");
        }

        private bool CommandHasHideAttribute(CommandInfo command)
        {
            return command.Attributes.Any(x => x.TypeId.ToString() == "Cake.Core.Discord.Attributes.HideAttribute");
        }
    }
}
