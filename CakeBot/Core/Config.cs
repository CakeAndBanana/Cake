using static CakeBot.Helper.JsonStorage.JsonConfig;

namespace CakeBot.Core
{
    public class Config
    {
        // Config file settings
        public static string ConnectionString;
        public static string OsuApi;
        public static string BotName;
        public static string BotPrefix;
        public static string BotStatus;
        public static string BotKey;
        public static string TwitterConsumerKey;
        public static string TwitterConsumerSecret;
        public static string TwitterAccessToken;
        public static string TwitterAccessTokenSecret;

        // Bot command settings
        public static bool SpamEnabled = true;
        public static bool LogEnabled = true;

        private static Config _instance;

        private Config()
        {
            GetConfigValues();
        }

        public static void InitConfig()
        {
            if (_instance == null)
            {
                _instance = new Config();
            }
        }

        public static void GetConfigValues()
        {
            var configValues = GetConfig();

            BotName = configValues.BotName;
            BotStatus = configValues.BotStatus;
            ConnectionString = configValues.ConnectionString;
#if DEBUG
            BotKey = configValues.DebugKey;
            BotPrefix = configValues.DebugPrefix;
#else
            BotKey = configValues.ReleaseKey;
#endif
            OsuApi = configValues.OsuApi;
            TwitterAccessToken = configValues.TwitterAccessToken;
            TwitterAccessTokenSecret = configValues.TwitterAccessTokenSecret;
            TwitterConsumerKey = configValues.TwitterConsumerKey;
            TwitterConsumerSecret = configValues.TwitterConsumerSecret;
        }
    }
}
