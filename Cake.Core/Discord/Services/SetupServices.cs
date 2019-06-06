using Cake.Modules.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Core.Discord.Services
{
    internal class SetupServices : ISetupServices
    {
        public ServiceProvider ReturnProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<CakeService>();
            return services.BuildServiceProvider();
        }
    }
}
