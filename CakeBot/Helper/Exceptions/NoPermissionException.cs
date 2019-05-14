using Discord;

namespace CakeBot.Helper.Exceptions
{
    public class NoPermissionException : CakeException
    {
        public NoPermissionException(IMentionable user) : base(user.Mention + ", you don't have permission to use this command!")
        {

        }
    }
}
