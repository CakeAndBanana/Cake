using System;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core.Discord.Attributes;
using Cake.Core.Discord.Embed.Builder;
using Cake.Core.Logging;
using Cake.Database.Queries;
using Discord.Commands;

namespace Cake.Core.Discord.Services
{
    public class HelpService : CustomBaseService
    {
        public async Task HelpAll(CommandService service)
        {
            var builder = new CakeEmbedBuilder(EmbedType.Info)
            {
                Title = "My commands: "
            };

            var isAdmin = UserQueries.FindOrCreateUser(Module.Context.User.Id).Result.Admin;
            var guild = await GuildQueries.FindOrCreateGuild(Module.Context.Guild.Id).ConfigureAwait(false);

            foreach (var module in service.Modules)
            {
                var showModule = false;
                string description = null;

                var moduleAttributes = module.Attributes.ToList();
                if (moduleAttributes.Find(m => m.TypeId.ToString() == "Cake.Core.Discord.Attributes.HideAttribute") == null)
                {
                    var preconditions = module.Preconditions.ToList();
                    foreach (var pre in preconditions)
                    {
                        if (isAdmin)
                        {
                            showModule = false;
                        }
                        else if (pre.TypeId.ToString() == "Cake.Core.Discord.Attributes.RequireAdminAttribute")
                        {
                            showModule = true;
                        }
                    }
                }
                else
                {
                    showModule = true;
                }

                if (!showModule)
                {
                    foreach (var command in module.Commands)
                    {
                        var showCommand = false;
                        var result = await command.CheckPreconditionsAsync(Module.Context);

                        var commandAttributes = command.Attributes.ToList();
                        if (commandAttributes.Find(m => m.Match(typeof(HideAttribute))) == null)
                        {
                            var conditions = command.Preconditions.ToList();
                            foreach (var pre in conditions)
                            {
                                if (isAdmin)
                                {
                                    showCommand = false;
                                }
                                else if (pre.TypeId.ToString() == "Cake.Core.Discord.Attributes.RequireAdminAttribute")
                                {
                                    showCommand = true;
                                }
                            }
                        }
                        else
                        {
                            showCommand = true;
                        }

                        var lastCommand = module.Commands.Last();
                        if (result.IsSuccess && !showCommand)
                        {
                            if (lastCommand == command || command.Name == "" || command.Module.Group == null)
                                switch (command.Name)
                                {
                                    case "" when lastCommand != command:
                                        description += $"__{guild.Prefix}{command.Module.Group}__ ``|`` ";
                                        break;
                                    case "" when lastCommand == command:
                                        description += $"__{guild.Prefix}{command.Module.Group}__";
                                        break;
                                    default:
                                    {
                                        if (command.Module.Group == null && lastCommand != command)
                                            description += $"__{guild.Prefix}{command.Name}__ ``|`` ";
                                        else if (command.Module.Group == null && lastCommand == command)
                                            description += $"__{guild.Prefix}{command.Name}__";
                                        else if (lastCommand == command)
                                            description += $"__{guild.Prefix}{command.Module.Group} {command.Name}__";
                                        break;
                                    }
                                }
                            else
                                description += $"__{guild.Prefix}{command.Module.Group} {command.Name}__ ``|`` ";
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(description) && !showModule)
                {
                    builder.AddField(x =>
                    {
                        x.Name = $"{module.Name}";
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            var dmChannel = await Module.Context.User.GetOrCreateDMChannelAsync();
            await SendEmbedAsync(builder, dmChannel);
        }

        public async Task HelpCommand(CommandService service, string command)
        {
            var guild = await GuildQueries.FindOrCreateGuild(Module.Context.Guild.Id).ConfigureAwait(false);
            var dmChannel = await Module.Context.User.GetOrCreateDMChannelAsync();
            var result = service.Search(Module.Context, command);
            if (!result.IsSuccess)
            {
                var builder1 = new CakeEmbedBuilder(EmbedType.Error)
                {

                    Title = "Error",
                    Description = $"the command: **{command}** doesn't exist.\n{guild.Prefix}help for all commands."
                };
                await dmChannel.SendMessageAsync("", false, builder1.Build());
                return;
            }

            var builder = new CakeEmbedBuilder(EmbedType.Info)
            {
                Description = $"Help about: **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;
                string preconditions = null;
                foreach (var precondition in cmd.Preconditions)
                {
                    preconditions += $"{precondition.TypeId}\n";
                }

                builder.AddField(x =>
                {
                    x.Name = "Command: " + string.Join(", ", cmd.Aliases);
                    x.Value = $"**Usage:** {cmd.Summary}\n" +
                              $"**Info:** {cmd.Remarks}\n" +
                              $"**Preconditions:** {preconditions}";
                    x.IsInline = false;
                });
            }

            await dmChannel.SendMessageAsync("", false, builder.Build());
        }
    }
}
