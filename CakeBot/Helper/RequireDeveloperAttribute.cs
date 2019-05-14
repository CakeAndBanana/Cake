using System;
using System.Linq;
using System.Threading.Tasks;
using CakeBot.Helper.Database.Queries;
using Discord;
using Discord.Commands;

namespace CakeBot.Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequireDeveloperAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            switch (context.Client.TokenType)
            {
                case TokenType.Bot:
                    await context.Client.GetApplicationInfoAsync();
                    if (UserQueries.CheckAdmin(context.User.Id).Result)
                    {
                        return PreconditionResult.FromSuccess();
                    }
                    return PreconditionResult.FromError("Command can only be run by the developers of the bot");
                default:
                    return PreconditionResult.FromError($"{nameof(RequireDeveloperAttribute)} is not supported by this {nameof(TokenType)}.");
            }
        }
    }
}
