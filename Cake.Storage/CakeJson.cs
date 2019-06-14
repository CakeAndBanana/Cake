﻿using System;
using System.IO;
using Cake.Storage.JsonModels;
using Newtonsoft.Json;

namespace Cake.Storage
{
    public class CakeJson
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
                BotKey = "Bot Key here", 
                LogEnabled = true,
                ConnectionString = null
            };

            File.WriteAllText(Path, JsonConvert.SerializeObject(model));
        }
    }
}
