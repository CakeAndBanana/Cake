using CakeBot.Helper;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using CakeBot.Helper.Modules.MAL;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeBot.Modules.Services
{
    public class MALService : CustomBaseService
    {
        public async Task GetRandomAnime(int genre_id)
        {
            try
            {
                var channel = (SocketTextChannel)Module.Context.Channel;
                var animes = MalData.GetRandomAnimeByCat(genre_id);
                if (animes == null)
                {
                    throw new CakeException("No Animes found.");
                }
                Random r = new Random();
                var anime = animes.anime[r.Next(animes.anime.Count())];
                if(!MalHelper.IsGoodScore(anime)) anime = animes.anime[r.Next(animes.anime.Count())];
                if (anime.r18 && !channel.IsNsfw) throw new CakeException("Not a NSFW Channel");
                await SendEmbedAsync(MalHelper.AnimeToEmbed(anime, false));
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        public async Task SearchForAnime(string animeName)
        {
            try
            {
                var channel = (SocketTextChannel)Module.Context.Channel;
                var animes = MalData.SearchAnime(animeName);
                if (animes.results.Count() == 0)
                {
                    throw new CakeException("No Animes found.");
                }
                var anime = animes.results[0];
                if (anime.r18 && !channel.IsNsfw) throw new CakeException("Not a NSFW Channel");
                await SendEmbedAsync(MalHelper.AnimeToEmbed(anime, true));
            }
            catch (CakeException e)
            {
                await SendMessageAsync(e.Message);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
    }
}
