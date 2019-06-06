using Cake.Core.Discord.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Core.Discord.Configuration
{
    public static class SetupServices
    {
        public static ServiceCollection ReturnProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<CakeService>();
            return services;
        }
    }
}
