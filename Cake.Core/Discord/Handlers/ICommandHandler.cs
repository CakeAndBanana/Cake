using System.Threading.Tasks;

namespace Cake.Core.Discord.Handlers
{
    interface ICommandHandler
    {
        Task InitializeAsync();
    }
}
