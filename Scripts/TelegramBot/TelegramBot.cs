using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;
using System.Threading;

namespace TextGameRPG.Scripts.TelegramBot
{
    public class TelegramBot
    {
        public static TelegramBot instance { get; private set; }

        public string dataPath { get; }
        public TelegramBotConfig config { get; private set; }
        public TelegramBotClient client { get; private set; }
        public TelegramBotReceiving botReceiving { get; private set; }
        public User mineUser { get; private set; }

        public bool isReceiving => botReceiving != null && botReceiving.isReceiving;

        public TelegramBot(string botDataPath)
        {
            instance = this;
            dataPath = botDataPath;
            config = GetConfig();
        }

        private TelegramBotConfig GetConfig()
        {
            string configPath = dataPath + "\\config.json";
            if (!System.IO.File.Exists(configPath))
            {
                var newConfig = new TelegramBotConfig();
                var jsonStr = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
                using (StreamWriter writer = new StreamWriter(configPath, false, Encoding.UTF8))
                {
                    writer.Write(jsonStr);
                }
                //MyConsole.LogWarning($"Created new bot config {configPath} \nYou need to enter a token!");
                return newConfig;
            }

            using (StreamReader reader = new StreamReader(configPath, Encoding.UTF8))
            {
                var jsonStr = reader.ReadToEnd();
                var config = JsonConvert.DeserializeObject<TelegramBotConfig>(jsonStr);
                return config;
            }
        }

        public async void StartAsync()
        {
            config = GetConfig(); //reload config: maybe something was changed before start
            //MyConsole.Log($"Starting {shortName} bot...");
            client = new TelegramBotClient(config.token);
            mineUser = await client.GetMeAsync();
            mineUser.CanJoinGroups = false;
            botReceiving = new TelegramBotReceiving(this);
            botReceiving.StartReceiving();
        }


    }
}
