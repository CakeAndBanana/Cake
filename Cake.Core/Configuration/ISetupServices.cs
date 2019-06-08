using System;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Core.Configuration
{
    interface ISetupServices
    {
        ServiceCollection ReturnProvider();
    }
}
