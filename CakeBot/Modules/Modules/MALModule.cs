using CakeBot.Helper.Modules.MAL;
using CakeBot.Modules.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeBot.Modules.Modules
{
    [Group("mal")]
    public class MALModule : CustomBaseModule
    {
        private readonly MALService _service;

        public MALModule(MALService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("anime r")]
        public async Task GetRandomAnime(int genre)
        {
            await _service.GetRandomAnime((MalAnimeGenreEnum)genre);
        }

        [Command("anime r")]
        public async Task GetRandomAnime([Remainder]string genre)
        {
            await _service.GetRandomAnime(MalEnumHelper.ParseEnum<MalAnimeGenreEnum>(genre));
        }

        [Command("anime s")]
        public async Task SearchForAnime([Remainder]string anime)
        {
            await _service.SearchForAnime(anime);
        }

        [Command("anime genres")]
        public async Task ListAnimeGenres()
        {
            await _service.SendAnimeGenres();
        }
        [Command("manga r")]
        public async Task GetRandommanga(int genre)
        {
            await _service.GetRandomManga((MalMangaGenreEnum)genre);
        }

        [Command("manga r")]
        public async Task GetRandommanga([Remainder]string genre)
        {
            await _service.GetRandomManga(MalEnumHelper.ParseEnum<MalMangaGenreEnum>(genre));
        }

        [Command("manga s")]
        public async Task SearchFormanga([Remainder]string manga)
        {
            await _service.SearchForManga(manga);
        }

        [Command("manga genres")]
        public async Task ListmangaGenres()
        {
            await _service.SendMangaGenres();
        }
    }
}
