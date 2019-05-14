using System;
using System.IO;
using CakeBot.Helper.JsonStorage.Model;
using Newtonsoft.Json;

namespace CakeBot.Helper.JsonStorage
{
    public class JsonConfig
    {
        private static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "config.json";
        public static ConfigModel GetConfig()
        {
            if (!File.Exists(Path))
                CreateConfig();
            return JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(Path));
        }

        private static void CreateConfig()
        {
            var model = new ConfigModel
            {
                BotName = "Cake",
                BotStatus = "Status Bot",
                DebugPrefix = "/",
                ReleaseKey = "Change me",
                DebugKey = "Change me",
                TillerinoApi = "Change me",
                OsuApi = "Change me",
                ConnectionString = "Change me",
                TwitterAccessToken = "Change me",
                TwitterAccessTokenSecret = "Change me",
                TwitterConsumerKey = "Change me",
                TwitterConsumerSecret = "Change me"
            };

            File.WriteAllText(Path, JsonConvert.SerializeObject(model));
        }
    }
}
