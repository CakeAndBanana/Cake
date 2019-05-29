using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.Core.Configuration
{
    class CakeConfiguration : ICakeConfiguration
    {
        public string BotToken { get; set; }
    }
}
