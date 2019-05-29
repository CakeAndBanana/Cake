using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.Core.Configuration
{
    public interface ICakeConfiguration
    {
        string BotToken { get; set; }
    }
}
