using Discord;

namespace CakeBot.Helper.Exceptions
{
    public class NoNsfwChannalException : CakeException
    {
        public NoNsfwChannalException(IMentionable user) : base(user.Mention + ", this command can only be executed in a NSFW channal!")
        {

        }
    }
}
