using CustomBaseModule = Cake.Core.Discord.Modules.CustomBaseModule;

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
