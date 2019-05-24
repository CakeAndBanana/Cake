using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeBot.Helper
{
    public class CakeCooldownModel
    {
        public class CooldownModel
        {
            public ulong Id { get; set; }
            public DateTime CooldownDateTime { get; set; }
        }
    }
}
