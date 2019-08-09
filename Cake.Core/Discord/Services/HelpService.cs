using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core.Discord.Attributes;
using Cake.Core.Discord.Embed.Builder;
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
                var hideModule = false;
                string description = null;

                var moduleAttributes = module.Attributes.ToList();
                if (moduleAttributes.Find(m => m.TypeId.ToString() == "Cake.Core.Discord.Attributes.HideAttribute") != null)
                {
                    var preconditions = module.Preconditions.ToList();
                    foreach (var pre in preconditions)
                    {
                        if (pre.TypeId.ToString() == "Cake.Core.Discord.Attributes.RequireBotAdminAttribute" || !isAdmin)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    hideModule = true;
                }


                if (hideModule)
                {
                    var commands = new List<CommandInfo>();
                    foreach (var command in module.Commands)
                    {
                        var showCommand = true;
                        var result = await command.CheckPreconditionsAsync(Module.Context);

                        var commandAttributes = command.Attributes.ToList();
                        if (commandAttributes.Find(m => m.Match(typeof(HideAttribute))) != null)
                        {
                            showCommand = false;
                        }

                        if (!result.IsSuccess || !showCommand) break;

                        commands.Add(command);
                    }

                    if (commands.Count > 0)
                    {
                        var lastCommand = commands.Last();
                        foreach (var command in commands)
                        {
                            if (lastCommand.Name == command.Name)
                                description += $"__{guild.Prefix}{command.Module.Group} {command.Name}__";
                            else
                                description += $"__{guild.Prefix}{command.Module.Group} {command.Name}__ ``|`` ";
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(description))
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
