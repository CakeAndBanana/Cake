using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cake.Modules.Services;
using Discord;
using Discord.Commands;

namespace Cake.Modules.Modules
{
    [Group("cake")]
    public class CakeModule : CustomBaseModule
    {
        private readonly CakeService _service;

        public CakeModule(CakeService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("status")]
        [Summary("cake status")]
        [RequireContext(ContextType.Guild)]
        [Alias("s")]
        [Remarks("Returns status of Cake.")]
        public async Task ReturnStatus()
        {
            await _service.GetStatus();
        }
    }
}
