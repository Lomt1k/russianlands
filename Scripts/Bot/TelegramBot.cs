using System.Text;
using System.IO;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TextGameRPG.Scripts.Bot
{
    using Sessions;
    using DataBase;
    using System.Threading.Tasks;
    using TextGameRPG.Scripts.GameCore.Managers;
    using TextGameRPG.Scripts.GameCore.Quests.Characters;
    using System.Net.Http;
    using TextGameRPG.Scripts.GameCore.Managers.Battles;

    public class TelegramBot
    {
        public static TelegramBot instance { get; private set; }
        public static HttpClient httpClient { get; } = new HttpClient();

        private BotConfig _config;

        public string dataPath { get; }
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
            _config = GetConfig();
        }

        private BotConfig GetConfig()
        {
            var jsonStr = string.Empty;
            string configPath = Path.Combine(dataPath, "config.json");
            if (!System.IO.File.Exists(configPath))
            {
                var newConfig = new BotConfig();
                jsonStr = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
                using (StreamWriter writer = new StreamWriter(configPath, false, Encoding.UTF8))
                {
                    writer.Write(jsonStr);
                }
                Program.logger.Warn($"Created new bot config {configPath} \nYou need to enter a token!");
                return newConfig;
            }

            BotConfig loadedConfig;
            using (StreamReader reader = new StreamReader(configPath, Encoding.UTF8))
            {
                jsonStr = reader.ReadToEnd();
                loadedConfig = JsonConvert.DeserializeObject<BotConfig>(jsonStr);                
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
            if (isReceiving)
            {
                Program.logger.Info("Bot listening is already started");
                return false;
            }

            Program.logger.Info($"Starting bot with data... {dataPath}");
            _config = GetConfig(); //reload config: maybe something was changed before start

            Program.logger.Info("Connecting to bot database...");
            dataBase = new BotDataBase(dataPath);
            bool isConnected = await dataBase.ConnectAsync();
            if (!isConnected)
            {
                Program.logger.Info($"Reject database connection!...");
                return false;
            }

            await WaitForNetworkConnection();
            client = new TelegramBotClient(_config.token);
            mineUser = await client.GetMeAsync();
            mineUser.CanJoinGroups = false;
            Program.SetTitle($"{mineUser.Username} [{dataPath}]");

            Singletones.OnBotStarted();
            messageSender = new MessageSender(client);
            sessionManager = new SessionManager(this);

            await CharacterStickersHolder.StickersUpdate();

            botReceiving = new TelegramBotReceiving(this);
            botReceiving.StartReceiving();

            return true;
        }

        public async Task StopListening()
        {
            if (!isReceiving)
            {
                Program.logger.Info("Bot listening is already stopped");
                return;
            }

            botReceiving.StopReceiving();
            await sessionManager.CloseAllSessions();
            await dataBase.CloseAsync();

            Singletones.OnBotStopped();
            messageSender = null;
            sessionManager = null;
            dataBase = null;
            botReceiving = null;
            client = null;
        }

        public async void Reconnect()
        {
            if (!isReceiving)
                return;

            Program.logger.Info("Reconnection starts...");
            botReceiving.StopReceiving();
            var battleManager = Singletones.Get<BattleManager>();
            if (battleManager != null)
            {
                var playersInBattle = battleManager.GetAllPlayers();
                battleManager.UnregisterAllBattles(); //специально вызываем перед закрытием сессий!
                foreach (var player in playersInBattle)
                {
                    await sessionManager.CloseSession(player.session.chatId, onError: true);
                }
            }
            await WaitForNetworkConnection();
            botReceiving.StartReceiving();
        }

        private async Task WaitForNetworkConnection()
        {
            while (true)
            {
                Program.logger.Debug("Connecting to telegram servers...");
                try
                {
                    using HttpRequestMessage checkConnectionRequest = new HttpRequestMessage(HttpMethod.Get, "https://t.me");
                    var result = await httpClient.SendAsync(checkConnectionRequest);
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
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
