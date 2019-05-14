using System;
using System.Collections.Generic;
using System.Threading;
using CakeBot.Helper.Modules.Background;

namespace CakeBot
{
    public class Program
    {
        private static readonly Startup Constant = GetInstance().Startup;

        private static void Main() => Constant.RunBotASync().GetAwaiter().GetResult();

        private static Program _instance; // Singleton instance

        public Startup Startup;

        public static Program GetInstance() // Singleton pattern
        {
            if (_instance != null) return _instance;
            _instance = new Program();
            _instance.Initialize();

            return _instance;
        }

        public void Initialize()
        {
            Startup = new Startup();
        }
    }

    public static class Global
    {
        internal static int baseXp = 125;
        internal static List<PurchasingUser> usersToTrack { get; set; } = new List<PurchasingUser>();
    }
}
