using CakeBot.Helper.Logging;

namespace CakeBot.Core
{
    class BotSettings
    {
        public static void SetupBotSettings()
        {
            Config.InitConfig();
            Logger.LogInt();
            Logger.LogInfo("~~ Starting ~~");
        }
    }
}
