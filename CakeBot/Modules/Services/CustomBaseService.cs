using System.Threading.Tasks;
using CakeBot.Helper;
using CakeBot.Helper.Logging;
using CakeBot.Modules.Modules;
using Discord;
using EmbedType = CakeBot.Helper.EmbedType;

namespace CakeBot.Modules.Services
{
    public class CustomBaseService
    {
        protected CustomBaseModule Module;

        public void SetBaseModule(CustomBaseModule module)
        {
            Module = module;
        }

        protected async Task SendMessageAsync(string message, bool tts = false, RequestOptions options = null)
        {
            await SendMessageAsync(message, Module.Context.Channel, tts, options);
        }

        protected async Task SendMessageAsync(string message, IMessageChannel channel, bool tts = false, RequestOptions options = null)
        {
            LogMessage(message, channel);
            await channel.SendMessageAsync(message, tts);
        }

        protected async Task SendEmbedAsync(CakeEmbedBuilder embed, bool tts = false, RequestOptions options = null)
        {
            await SendEmbedAsync(embed, Module.Context.Channel, tts, options);
        }

        protected async Task SendEmbedAsync(CakeEmbedBuilder embed, IMessageChannel channel, bool tts = false, RequestOptions options = null)
        {
            LogEmbed(embed, channel);
            await channel.SendMessageAsync("", tts, embed.Build(), options);
        }
        private void LogMessage(string message, IMessageChannel channel)
        {
            Logger.LogInfo("Sending message '" +  message + "' to channel" + channel.Id);
        }

        private void LogEmbed(CakeEmbedBuilder embedBuilder, IMessageChannel channel)
        {
            var message = "Sending " + embedBuilder.EmbedType + " embed to channel " + channel.Id;

            switch (embedBuilder.EmbedType)
            {
                case EmbedType.None:
                    Logger.LogInfo(message);
                    break;
                case EmbedType.Debug:
                    Logger.LogDebug(message);
                    break;
                case EmbedType.Info:
                    Logger.LogInfo(message);
                    break;
                case EmbedType.Success:
                    Logger.LogSuccess(message);
                    break;
                case EmbedType.Warning:
                    Logger.LogWarning(message);
                    break;
                case EmbedType.Error:
                    Logger.LogError(message);
                    break;
                default:
                    Logger.LogInfo(message);
                    break;
            }
        }
    }
}
