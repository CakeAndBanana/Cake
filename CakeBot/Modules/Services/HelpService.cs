using System;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Core;
using CakeBot.Core.Helper;
using CakeBot.Helper;
using CakeBot.Helper.Database.Queries;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using Discord.Commands;

namespace CakeBot.Modules.Services
{
    public class HelpService : CustomBaseService
    {
        public async Task HelpAll(CommandService service)
        {
            try
            {
                var builder = new CakeEmbedBuilder(EmbedType.Info)
                {
                    Title = "My commands: "
                };

                var isAdmin = await UserQueries.CheckAdmin(Module.Context.User.Id);
                var guildPrefix = await GuildQueries.FindPrefix(PrefixHelper.GuildIdFinder(Startup.GetClient(), Module.Context.Message.Channel.Id));

                foreach (var module in service.Modules)
                {
                    var showModule = false;
                    string description = null;
                    var preconditions = module.Preconditions.ToList();

                    foreach (var pre in preconditions)
                    {
                        if (isAdmin)
                        {
                            showModule = false;
                        }
                        else if (pre.TypeId.ToString() == "CakeBot.Helper.RequireDeveloperAttribute")
                        {
                            showModule = true;
                        }
                    }

                    foreach (var command in module.Commands)
                    {
                        var showCommand = false;
                        var result = await command.CheckPreconditionsAsync(Module.Context);
                        var conditions = command.Preconditions.ToList();

                        foreach (var pre in conditions)
                        {
                            if (isAdmin)
                            {
                                showCommand = false;
                            }
                            else if (pre.TypeId.ToString() == "CakeBot.Helper.RequireDeveloperAttribute")
                            {
                                showCommand = true;
                            }
                        }

                        var lastCommand = module.Commands.Last();
                        if (result.IsSuccess && !showCommand)
                        {
                            if (lastCommand != command && command.Name != "" && command.Module.Group != null)
                                description += $"__{guildPrefix}{command.Module.Group} {command.Name}__ ``|`` ";
                            else if (command.Name == "" && lastCommand != command)
                                description += $"__{guildPrefix}{command.Module.Group}__ ``|`` ";
                            else if (command.Name == "" && lastCommand == command)
                                description += $"__{guildPrefix}{command.Module.Group}__";
                            else if (command.Module.Group == null && lastCommand != command)
                                description += $"__{guildPrefix}{command.Name}__ ``|`` ";
                            else if (command.Module.Group == null && lastCommand == command)
                                description += $"__{guildPrefix}{command.Name}__";
                            else if (lastCommand == command)
                                description += $"__{guildPrefix}{command.Module.Group} {command.Name}__";
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
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public async Task HelpCommand(CommandService service, string command)
        {
            try
            {
                var dmChannel = await Module.Context.User.GetOrCreateDMChannelAsync();
                var result = service.Search(Module.Context, command);
                if (!result.IsSuccess)
                {
                    var builder1 = new CakeEmbedBuilder(EmbedType.Error)
                    {

                        Title = "Error",
                        Description = $"the command: **{command}** doesn't exist.\n{Config.BotPrefix}help for all commands."
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

                    builder.AddField(x =>
                    {
                        x.Name = "Command: " + string.Join(", ", cmd.Aliases);
                        x.Value = $"**Usage:** {cmd.Summary}\n" +
                                  $"**Info:** {cmd.Remarks}";
                        x.IsInline = false;
                    });
                }

                await dmChannel.SendMessageAsync("", false, builder.Build());
            }
            catch (CakeException e)
            {
                var embedError = e.GetEmbededError();
                await SendEmbedAsync(embedError);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
    }
}
