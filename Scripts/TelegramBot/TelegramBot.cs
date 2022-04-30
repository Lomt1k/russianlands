using System.Text;
using System.IO;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TextGameRPG.Scripts.TelegramBot
{
    using Sessions;
    using DataBase;
    using System.Threading.Tasks;

    public class TelegramBot
    {
        public static TelegramBot instance { get; private set; }

        public string dataPath { get; }
        public TelegramBotConfig config { get; private set; }
        public TelegramBotClient client { get; private set; }
        public User mineUser { get; private set; }
        public BotDataBase dataBase { get; private set; }
        public SessionManager sessionManager { get; private set; }
        public TelegramBotReceiving botReceiving { get; private set; }

        public bool isReceiving => botReceiving != null && botReceiving.isReceiving;

        public TelegramBot(string botDataPath)
        {
            instance = this;
            dataPath = botDataPath;
        }

        public void Init()
        {
            config = GetConfig();
        }

        private TelegramBotConfig GetConfig()
        {
            string configPath = Path.Combine(dataPath, "config.json");
            if (!System.IO.File.Exists(configPath))
            {
                var newConfig = new TelegramBotConfig();
                var jsonStr = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
                using (StreamWriter writer = new StreamWriter(configPath, false, Encoding.UTF8))
                {
                    writer.Write(jsonStr);
                }
                Program.logger.Warn($"Created new bot config {configPath} \nYou need to enter a token!");
                return newConfig;
            }

            using (StreamReader reader = new StreamReader(configPath, Encoding.UTF8))
            {
                var jsonStr = reader.ReadToEnd();
                var config = JsonConvert.DeserializeObject<TelegramBotConfig>(jsonStr);
                return config;
            }
        }

        public async Task<bool> StartListeningAsync()
        {
            config = GetConfig(); //reload config: maybe something was changed before start
            Program.logger.Info($"Starting bot with data... {dataPath}");
            client = new TelegramBotClient(config.token);
            mineUser = await client.GetMeAsync();
            mineUser.CanJoinGroups = false;
            Program.mainWindow.Title = $"{mineUser.Username} [{dataPath}]";

            dataBase = new BotDataBase(dataPath);
            bool isConnected = await dataBase.Connect();
            if (!isConnected)
            {
                return false;
            }

            sessionManager = new SessionManager(this);
            botReceiving = new TelegramBotReceiving(this);
            botReceiving.StartReceiving();

            return true;
        }

        public void StopListening()
        {
            botReceiving.StopReceiving();
        }


    }
}
