using System.Threading.Tasks;
using Cake.Core.Discord.Services;
using Discord.Commands;

namespace Cake.Core.Discord.Modules
{
    [Group("cake")]
    [Name("Cake")]
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
        [Alias("s")]
        [Remarks("Returns status of Cake.")]
        public async Task ReturnStatus()
        {
            await _service.GetStatus();
        }

        [Command("prefix")]
        [Summary("cake prefix (newprefix?)")]
        [Alias("p")]
        [Remarks("Returns or set the prefix of Cake.")]
        public async Task ReturnPrefix(string newprefix = null)
        {
            if (newprefix == null)
            {
                await _service.GetPrefix();
            }
            else
            {
;               await _service.SetPrefix(newprefix);
            }
        }
    }
}
