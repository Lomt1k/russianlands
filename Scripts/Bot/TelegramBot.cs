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
        private static readonly SessionManager sessionManager = Singletones.Get<SessionManager>();
        private static readonly PerformanceManager performanceManager = Singletones.Get<PerformanceManager>();

        public static TelegramBot instance { get; private set; }
        public static HttpClient httpClient { get; } = new HttpClient();

        private BotConfig _config;
        private TelegramBotReceiving _botReceiving;

        public string dataPath { get; }
        public TelegramBotClient botClient { get; private set; }
        public User mineUser { get; private set; }
        public BotDataBase dataBase { get; private set; }
        public MessageSender messageSender { get; private set; }

        public bool isReceiving => _botReceiving != null && _botReceiving.isReceiving;

        public TelegramBot(string botDataPath)
        {
            instance = this;
            dataPath = botDataPath;

            _config = GetConfig();
            _botReceiving = new TelegramBotReceiving(this);

            dataBase = new BotDataBase(botDataPath);
            botClient = new TelegramBotClient(_config.token);
            messageSender = new MessageSender(botClient);
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
            Program.logger.Info("Connecting to bot database...");
            bool isConnected = await dataBase.ConnectAsync();
            if (!isConnected)
            {
                Program.logger.Info($"Reject database connection!...");
                return false;
            }

            await WaitForNetworkConnection();
            mineUser = await botClient.GetMeAsync();
            mineUser.CanJoinGroups = false;
            Program.SetTitle($"{mineUser.Username} [{dataPath}]");

            SubcribeEvents();
            Singletones.OnBotStarted();

            await CharacterStickersHolder.StickersUpdate();

            _botReceiving.StartReceiving();

            return true;
        }

        public async Task StopListening()
        {
            if (!isReceiving)
                return;

            _botReceiving.StopReceiving();
            await sessionManager.CloseAllSessions();
            await dataBase.CloseAsync();

            Singletones.OnBotStopped();
            UnsubscribeEvents();
        }

        public async void Reconnect()
        {
            if (!isReceiving)
                return;

            Program.logger.Info("Reconnection starts...");
            _botReceiving.StopReceiving();
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
            _botReceiving.StartReceiving();
        }

        private async Task WaitForNetworkConnection()
        {
            while (true)
            {
                Program.logger.Info("Connecting to telegram servers...");
                try
                {
                    using HttpRequestMessage checkConnectionRequest = new HttpRequestMessage(HttpMethod.Get, "https://t.me");
                    var result = await httpClient.SendAsync(checkConnectionRequest);
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Program.logger.Info($"Connected to telegram servers");
                        return; //success!
                    }
                }
                catch (System.Exception ex)
                {
                    //ignored
                }
                await Task.Delay(1000);
            }
        }

        private void SubcribeEvents()
        {
            performanceManager.onStateUpdate += OnUpdatePerformanceState;
        }

        private void UnsubscribeEvents()
        {
            performanceManager.onStateUpdate -= OnUpdatePerformanceState;
        }

        private async void OnUpdatePerformanceState(PerformanceState state)
        {
            if (isReceiving && state == PerformanceState.ShutdownRequired)
            {
                Shutdown(fatalError: "Performance limit reached");
            }
        }

        public async void Shutdown(string? fatalError = null)
        {
            if (fatalError != null)
            {
                Program.logger.Fatal("The server will be shutdown. Reason:" + fatalError);
            }
            else
            {
                Program.logger.Info("The server will be shutdown...");
            }
            Program.logger.Info("Performance Stats:\n" + performanceManager.debugInfo);
            
            await StopListening();
            Program.logger.Info("Stopping the server was completed correctly");
            System.Environment.Exit(0);
        }


    }
}
