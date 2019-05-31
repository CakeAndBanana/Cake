using System;
using System.Collections.Generic;
using System.Text;
using Cake.Modules.Modules;

namespace Cake.Modules.Services
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
