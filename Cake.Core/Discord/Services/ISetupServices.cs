using System;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Core.Discord.Services
{
    internal interface ISetupServices
    {
        ServiceProvider ReturnProvider();
    }
}
