using Discord;

namespace CakeBot.Helper.Exceptions
{
    public class NoSelfCommandException : CakeException
    {
        public NoSelfCommandException(IMentionable user) : base("You cannot use this command on yourself " + user.Mention)
        {

        }
    }
}
