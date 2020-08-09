using Cake.Core.Discord.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Core.Configuration
{
    public class SetupServices : ISetupServices
    {
        public ServiceCollection ReturnProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<CakeService>();
            services.AddSingleton<HelpService>();
            services.AddSingleton<OsuService>();
            services.AddSingleton<UserService>();
            return services;
        }
    }
}