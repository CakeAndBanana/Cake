using Cake.Core.Discord.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Core.Discord.Modules
{
    public class UserModule : CustomBaseModule
    {
        private readonly UserService _service;

        public UserModule(UserService service)
        {
            _service = service;
            _service.SetBaseModule(this);
        }

        [Command("experience")]
        [Summary("cake xp")]
        [Alias("xp")]
        [Remarks("Returns your experience in Cake.")]
        public async Task ReturnUserExperience()
        {
            await _service.GetUserExperience();
        }
    }
}
