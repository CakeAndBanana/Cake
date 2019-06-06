using Cake.Core.Discord.Modules;

namespace Cake.Core.Discord.Services
{
    public class CustomBaseService
    {
        protected CustomBaseModule Module;

        public void SetBaseModule(CustomBaseModule module)
        {
            Module = module;
        }
    }
}
