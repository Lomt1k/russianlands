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
    using TextGameRPG.Scripts.GameCore.Quests.Characters;
    using System.Net.Http;
    using System.Linq;

    public class TelegramBot
    {
        public static TelegramBot instance { get; private set; }
        public static HttpClient httpClient { get; } = new HttpClient();

        public string dataPath { get; }
        public TelegramBotConfig config { get; private set; }
        public TelegramBotClient client { get; private set; }
        public User mineUser { get; private set; }
        public BotDataBase dataBase { get; private set; }
        public MessageSender messageSender { get; private set; }
        public SessionManager sessionManager { get; private set; }
        public TelegramBotReceiving botReceiving { get; private set; }

        public bool isReceiving => botReceiving != null && botReceiving.isReceiving;
        public bool isRestarting { get; private set; }

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

        public async Task<bool> StartListening()
        {
            Program.logger.Info($"Starting bot with data... {dataPath}");
            await WaitForNetworkConnection();
            config = GetConfig(); //reload config: maybe something was changed before start
            client = new TelegramBotClient(config.token);
            mineUser = await client.GetMeAsync();
            mineUser.CanJoinGroups = false;
            if (!isRestarting)
            {
                Program.mainWindow.Title = $"{mineUser.Username} [{dataPath}]"; // без этого тут зависает при рестарте!
            }

            dataBase = new BotDataBase(dataPath);
            bool isConnected = await dataBase.Connect();
            if (!isConnected)
            {
                Program.logger.Info($"Reject database connection!...");
                return false;
            }

            GlobalManagers.CreateManagers();
            messageSender = new MessageSender(client);
            sessionManager = new SessionManager(this);

            await CharacterStickersHolder.StickersUpdate();

            botReceiving = new TelegramBotReceiving(this);
            botReceiving.StartReceiving();

            return true;
        }

        public async Task StopListening()
        {
            if (botReceiving == null)
                return;

            botReceiving.StopReceiving();
            await sessionManager.CloseAllSessions();

            GlobalManagers.DisposeManagers();
            messageSender = null;
            sessionManager = null;
            botReceiving = null;
        }

        public async Task Restart(bool withNotifyUsers = true)
        {
            if (isRestarting)
                return;

            Program.logger.Info("Restarting...");
            isRestarting = true;
            var allActiveChats = sessionManager.GetAllChats().Select(x => x.Identifier).ToList();

            await StopListening();
            await StartListening();
            isRestarting = false;

            if (withNotifyUsers)
            {
                foreach (var chatId in allActiveChats)
                {
                    Program.logger.Info($"Restart notification for {chatId}");
                    await messageSender.SendTextMessage(chatId.Value, "The bot has been restarted due to an unstable internet connection.\n\nNow you can continue playing!");
                }
            }
        }

        private async Task WaitForNetworkConnection()
        {
            Program.logger.Debug("Waiting for connection with telegram servers...");
            while (true)
            {
                try
                {
                    using HttpRequestMessage checkConnectionRequest = new HttpRequestMessage(HttpMethod.Get, "https://t.me");
                    var result = await httpClient.SendAsync(checkConnectionRequest);
                    if (result.IsSuccessStatusCode && result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Program.logger.Debug($"Status Code: {result.StatusCode}");
                        return; //success!
                    }
                }
                catch (System.Exception ex)
                {
                    //Program.logger.Info(ex.Message);
                }                
                await Task.Delay(1000);
            }
        }


    }
}
