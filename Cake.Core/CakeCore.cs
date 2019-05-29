using System.Threading.Tasks;
using Cake.Core.Configuration;

namespace Cake.Core
{
    public class CakeCore
    {
        public ICakeDiscord Cake { get; }

        public ICakeConfiguration CakeConfiguration { get; }

        public CakeCore(ICakeDiscord cake, ICakeConfiguration cakeConfiguration)
        {
            Cake = cake;
            CakeConfiguration = cakeConfiguration;
        }

        public async Task StartASync()
        {
            await Cake.StartASync();
        }
    }
}
