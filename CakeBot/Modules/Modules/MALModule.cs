using CakeBot.Helper.Modules.MAL;
using CakeBot.Modules.Services;
using Discord.Commands;
using System.Threading.Tasks;

namespace CakeBot.Modules.Modules
{
    [Name("MyAnimeList")]
    [Group("myanimelist")]
    [Alias("mal")]
    public class MalModule : CustomBaseModule
    {
        private readonly MalService _service;

        public MalModule(MalService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("anime random")]
        [Summary(">anime random (number or name of genre)")]
        [Remarks("Returns a random anime from MyAnimeList.net")]
        [Alias("anime r")]
        public async Task GetRandomAnime(int genre)
        {
            await _service.GetRandomAnime((MalAnimeGenreEnum)genre);
        }

        [Command("anime random")]
        [Summary(">anime random (number or name of genre)")]
        [Remarks("Returns a random anime from MyAnimeList.net")]
        [Alias("anime r")]
        public async Task GetRandomAnime([Remainder]string genre)
        {
            await _service.GetRandomAnime(MalEnumHelper.ParseEnum<MalAnimeGenreEnum>(genre));
        }

        [Command("anime search")]
        [Summary(">anime search (number or name of genre)")]
        [Remarks("Search for an anime on MyAnimeList.net")]
        [Alias("anime s")]
        public async Task SearchForAnime([Remainder]string anime)
        {
            await _service.SearchForAnime(anime);
        }

        [Command("anime genres")]
        [Summary(">anime genres")]
        [Remarks("Returns a list of genres to use with anime random")]
        [Alias("anime g")]
        public async Task ListAnimeGenres()
        {
            await _service.SendAnimeGenres();
        }

        [Command("manga random")]
        [Summary(">manga random (number or name of genre)")]
        [Remarks("Returns a random manga from MyAnimeList.net")]
        [Alias("manga r")]
        public async Task GetRandomManga(int genre)
        {
            await _service.GetRandomManga((MalMangaGenreEnum)genre);
        }

        [Command("manga random")]
        [Summary(">manga random (number or name of genre)")]
        [Remarks("Returns a random manga from MyAnimeList.net")]
        [Alias("manga r")]
        public async Task GetRandomManga([Remainder]string genre)
        {
            await _service.GetRandomManga(MalEnumHelper.ParseEnum<MalMangaGenreEnum>(genre));
        }

        [Command("manga search")]
        [Summary(">manga search (name of manga)")]
        [Remarks("Search for a manga on MyAnimeList.net")]
        [Alias("manga s")]
        public async Task SearchForManga([Remainder]string manga)
        {
            await _service.SearchForManga(manga);
        }

        [Command("manga genres")]
        [Summary(">manga genres")]
        [Remarks("Returns a list of genres to use with manga random")]
        [Alias("manga g")]
        public async Task ListMangaGenres()
        {
            await _service.SendMangaGenres();
        }
    }
}
