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
    [Group("anime")]
    public class MALModule : CustomBaseModule
    {
        private readonly MALService _service;

        public MALModule(MALService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("r")]
        public async Task GetRandomAnime(int genre)
        {
            await _service.GetRandomAnime((MalAnimeGenreEnum)genre);
        }

        [Command("r")]
        public async Task GetRandomAnime([Remainder]string genre)
        {
            await _service.GetRandomAnime(MalEnumHelper.ParseEnum<MalAnimeGenreEnum>(genre));
        }

        [Command("s")]
        public async Task SearchForAnime([Remainder]string anime)
        {
            await _service.SearchForAnime(anime);
        }

        [Command("genres")]
        public async Task ListAnimeGenres()
        {
            await _service.SendAnimeGenres();
        }
    }
}
