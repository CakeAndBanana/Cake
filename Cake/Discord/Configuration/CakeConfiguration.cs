using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Discord.Configuration
{
    class CakeConfiguration : ICakeConfiguration
    {
        public string BotKey { get; set; }
    }
}
