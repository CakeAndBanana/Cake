using System.Threading.Tasks;

namespace Cake.Core
{
    interface ICake
    {
        Task StartAsync();

        void SetupBot();
    }
}
