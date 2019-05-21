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
    public class MALModule : CustomBaseModule
    {
        private readonly MALService _service;

        public MALModule(MALService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("anime -r")]
        public async Task GetRandomAnime(int genre)
        {
            await _service.GetRandomAnime(genre);
        }
        [Command("anime -s")]
        public async Task GetRandomAnime([Remainder]string anime)
        {
            await _service.SearchForAnime(anime);
        }
    }
}
