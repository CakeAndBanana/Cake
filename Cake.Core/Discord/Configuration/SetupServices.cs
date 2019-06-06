using Cake.Core.Discord.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Core.Discord.Configuration
{
    public static class SetupServices
    {
        public static ServiceProvider ReturnProvider()
        {
            var services = new ServiceCollection()
                .AddSingleton<CakeService>();
            return services.BuildServiceProvider();
        }
    }
}
