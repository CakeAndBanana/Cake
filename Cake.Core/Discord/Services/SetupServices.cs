using System;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Core.Discord.Services
{
    public class SetupServices : ISetupServices
    {
        public IServiceProvider ReturnProvider()
        {
            var services = new ServiceCollection();
            // Add Modules here
            return services.BuildServiceProvider();
        }
    }
}
