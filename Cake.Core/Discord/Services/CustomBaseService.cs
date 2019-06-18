using System.Threading.Tasks;
using Cake.Core.Discord.Embed;
using Cake.Core.Discord.Embed.Builder;
using Cake.Core.Discord.Modules;
using Cake.Core.Logging;
using Discord;

namespace Cake.Core.Discord.Services
{
    public class CustomBaseService
    {
        protected static CustomBaseModule Module;

        public void SetBaseModule(CustomBaseModule module)
        {
            Module = module;
        }
        protected async Task SendMessageAsync(string message, bool tts = false, RequestOptions options = null)
        {
            await SendMessageAsync(message, Module.Context.Channel, tts, options).ConfigureAwait(false);
        }
        protected async Task SendMessageAsync(string message, IMessageChannel channel, bool tts = false, RequestOptions options = null)
        {
            await channel.SendMessageAsync(message, tts).ConfigureAwait(false);
        }

        protected async Task SendEmbedAsync(CakeEmbedBuilder embed, bool tts = false, RequestOptions options = null)
        {
            await SendEmbedAsync(embed, Module.Context.Channel, tts, options).ConfigureAwait(false);
        }

        protected async Task SendEmbedAsync(CakeEmbedBuilder embed, IMessageChannel channel, bool tts = false, RequestOptions options = null)
        {
            await channel.SendMessageAsync("", tts, embed.Build(), options).ConfigureAwait(false);
        }
    }
}
