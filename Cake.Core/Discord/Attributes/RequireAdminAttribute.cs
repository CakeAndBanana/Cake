﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Cake.Core.Discord.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    class RequireAdminAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            switch (context.Client.TokenType)
            {
                case TokenType.Bot:
                    await context.Client.GetApplicationInfoAsync();
                    if (Database.Queries.UserQueries.FindOrCreateUser(context.User.Id).Result.Admin)
                    {
                        return PreconditionResult.FromSuccess();
                    }
                    return PreconditionResult.FromError("Command can only be used by bot admins.");
                default:
                    return PreconditionResult.FromError($"{nameof(RequireAdminAttribute)} is not supported by this {nameof(TokenType)}.");
            }
        }
    }
}
