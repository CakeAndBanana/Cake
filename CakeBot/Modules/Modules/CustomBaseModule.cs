using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace CakeBot.Modules.Modules
{
    public class CustomBaseModule : ModuleBase
    {
        public async Task ServiceReplyAsync(string s)
        {
            await ReplyAsync(s);
        }

        public async Task ServiceReplyAsync(string title, Embed emb)
        {
            await ReplyAsync(title, false, emb);
        }
    }
}
