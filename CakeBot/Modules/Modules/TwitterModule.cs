using System.Threading.Tasks;
using CakeBot.Modules.Services;
using Discord.Commands;

namespace CakeBot.Modules.Modules
{
    [Group("twitter")]
    [Alias("tw")]
    [Name("Twitter")]
    public class TwitterModule : CustomBaseModule
    {
        private readonly TwitterService _service;

        public TwitterModule(TwitterService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("list")]
        [Remarks(">twitter list")]
        [Summary("List of twitter account linked to the channel")]
        [Alias("l")]
        public async Task TwitterList()
        {
            await _service.ListTwitter();
        }

        [Command("status")]
        [Remarks(">twitter status")]
        [Summary("Status of TweetBot")]
        [Alias("s")]
        public async Task TwitterStatus()
        {
            await _service.StatusTwitter();
        }

        [Command("add")]
        [Remarks(">twitter add (username)")]
        [Summary("Link twitter user to the specified channel (max 3).")]
        [Alias("a")]
        public async Task TwitterAdd([Remainder]string username)
        {
            await _service.AddTwitter(username);
        }

        [Command("remove")]
        [Remarks(">twitter remove (id)")]
        [Summary("Remove twitter user to the specified channel.")]
        [Alias("r")]
        public async Task TwitterRemove([Remainder]string username)
        {
            await _service.RemoveTwitter(username);
        }
    }
}