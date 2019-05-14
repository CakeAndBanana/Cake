using CakeBot.Core.Helper;
using Discord.Commands;

namespace CakeBot.Helper.Exceptions
{
    public class BadCommandUsageException : CakeException
    {
        public BadCommandUsageException(ModuleInfoHelper helper, CommandContext context) 
            : base($"{ context.User.Mention }, you did not use the command correctly!\n**Usage: **{ helper.GetCommand(context.Message).Remarks }" )
        {

        }

    }
}
