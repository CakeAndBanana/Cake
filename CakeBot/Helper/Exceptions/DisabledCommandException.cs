using Discord;

namespace CakeBot.Helper.Exceptions
{
    public class DisabledCommandException : CakeException
    {
        public DisabledCommandException(IMentionable user) : base(user.Mention + ", this command is disabled!")
        {

        }
    }
}
