using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Discord.Configuration
{
    interface ICakeConfiguration
    {
        string BotKey { get; }
    }
}
