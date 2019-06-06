using System.Threading.Tasks;

namespace Cake.Core.Discord.Handlers
{
    internal interface ICommandHandler
    {
        Task InitializeAsync();
    }
}
