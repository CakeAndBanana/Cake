using System;
using CakeBot.Modules.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CakeBot.Core.Services
{
    public class SetupServices
    {
        public static IServiceProvider ReturnProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<AdminService>();
            services.AddSingleton<CakeService>();
            services.AddSingleton<GamesService>();
            services.AddSingleton<TwitterService>();
            services.AddSingleton<OsuService>();
            services.AddSingleton<HelpService>();
            services.AddSingleton<FishService>();
            services.AddSingleton<EconomyService>();
            services.AddSingleton<ProfileService>();
            services.AddSingleton<MalService>();

            return services.BuildServiceProvider();
        }
    }
}
