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
    using TextGameRPG.Scripts.TelegramBot.Managers;

    public class TelegramBot
    {
        public static TelegramBot instance { get; private set; }

        public string dataPath { get; }
        public TelegramBotConfig config { get; private set; }
        public TelegramBotClient client { get; private set; }
        public User mineUser { get; private set; }
        public BotDataBase dataBase { get; private set; }
        public MessageSender messageSender { get; private set; }
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
            var jsonStr = string.Empty;
            string configPath = Path.Combine(dataPath, "config.json");
            if (!System.IO.File.Exists(configPath))
            {
                var newConfig = new TelegramBotConfig();
                jsonStr = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
                using (StreamWriter writer = new StreamWriter(configPath, false, Encoding.UTF8))
                {
                    writer.Write(jsonStr);
                }
                Program.logger.Warn($"Created new bot config {configPath} \nYou need to enter a token!");
                return newConfig;
            }

            TelegramBotConfig loadedConfig;
            using (StreamReader reader = new StreamReader(configPath, Encoding.UTF8))
            {
                jsonStr = reader.ReadToEnd();
                loadedConfig = JsonConvert.DeserializeObject<TelegramBotConfig>(jsonStr);                
            }

            // пересохраняем загруженный конфиг (на случай, если в новой версии приложения были добавлены новые поля - чтобы они появились)
            jsonStr = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(configPath, false, Encoding.UTF8))
            {
                writer.Write(jsonStr);
            }

            return loadedConfig;
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

            GlobalManagers.CreateManagers();
            messageSender = new MessageSender(client);
            sessionManager = new SessionManager(this);
            botReceiving = new TelegramBotReceiving(this);
            botReceiving.StartReceiving();

            return true;
        }

        public async Task StopListening()
        {
            botReceiving.StopReceiving();
            await sessionManager.CloseAllSessions();
            GlobalManagers.DisposeManagers();
        }


    }
}
