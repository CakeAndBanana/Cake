using CakeBot.Helper;
using CakeBot.Helper.Exceptions;
using CakeBot.Helper.Logging;
using CakeBot.Helper.Modules.MAL;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CakeBot.Modules.Services
{
    public class MalService : CustomBaseService
    {
        public async Task GetRandomAnime(MalAnimeGenreEnum genre)
        {
            try
            {
                var channel = (SocketTextChannel)Module.Context.Channel;
                var animes = MalData.GetRandomAnimeByCat((int)genre);

                if (animes == null)
                {
                    throw new CakeException("No Anime(s) found.");
                }

                var r = new Random();
                var anime = animes.anime[r.Next(animes.anime.Count())];

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
        public async Task GetRandomManga(MalMangaGenreEnum genre)
        {
            try
            {
                var mangaList = MalData.GetRandomMangaByCat((int)genre);
                if (mangaList == null)
                {
                    throw new CakeException("No Manga(s) found.");
                }

                var r = new Random();
                var manga = mangaList.manga[r.Next(mangaList.manga.Count())];

                await SendEmbedAsync(MalHelper.MangaToEmbed(manga));
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

                if (animes == null)
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
        public async Task SearchForManga(string mangaName)
        {
            try
            {
                var mangaList = MalData.SearchManga(mangaName);
                if (mangaList == null)
                {
                    throw new CakeException("No Manga(s) found.");
                }

                var manga = mangaList.results[0];
                await SendEmbedAsync(MalHelper.MangaToEmbed(manga));
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
        public async Task SendAnimeGenres()
        {
            try
            {
                var values = (MalAnimeGenreEnum[])Enum.GetValues(typeof(MalAnimeGenreEnum));

                var description = values.Aggregate<MalAnimeGenreEnum, string>(null, (current, value) => current + $"{value} | {(int) value}\n");

                var embedList = new CakeEmbedBuilder()
                    .WithTitle("List of Manga Genres")
                    .WithDescription(description) as CakeEmbedBuilder;
                await SendEmbedAsync(embedList);
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
        public async Task SendMangaGenres()
        {
            try
            {
                var values = (MalMangaGenreEnum[])Enum.GetValues(typeof(MalMangaGenreEnum));

                var description = values.Aggregate<MalMangaGenreEnum, string>(null, (current, value) => current + $"{value} | {(int)value}\n");

                var embedList = new CakeEmbedBuilder()
                    .WithTitle("List of Anime Genres")
                    .WithDescription(description) as CakeEmbedBuilder;
                await SendEmbedAsync(embedList);
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
