using System;
using System.IO;
using Cake.Json.JsonModels;
using Newtonsoft.Json;

namespace Cake.Json
{
    public class CakeJson
    {
        private static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "config.json";
        public static ConfigModel GetConfig()
        {
            ConfigModel config;
            if (!File.Exists(Path))
            {
                config = CreateConfig();
            }
            else
            {
                config = GetConfigModel();
            }

            if (config.Version != System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                File.Delete(Path);
                config.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                config = CreateConfig(config);
            }

            return config;
        }

        private static ConfigModel CreateConfig(ConfigModel model = null)
        {
            if (model == null)
            {
                model = new ConfigModel
                {
                    Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    BotKey = "Bot Key here",
                    OsuApiKey = "Osu Api Here",
                    LogEnabled = true,
                    ConnectionString = null, 
                    DatabaseProvider = "SqlServer"
                };
            }

            File.WriteAllText(Path, JsonConvert.SerializeObject(model));
            return JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(Path));
        }

        private static ConfigModel GetConfigModel() => JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(Path));
    }
}
