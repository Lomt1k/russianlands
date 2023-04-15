using System.Text;
using System.IO;
using Newtonsoft.Json;
using Telegram.Bot;

namespace TextGameRPG.Scripts.Bot
{
    using Sessions;
    using DataBase;
    using System.Threading.Tasks;
    using TextGameRPG.Scripts.GameCore.Services;
    using TextGameRPG.Scripts.GameCore.Quests.Characters;
    using System.Net.Http;
    using TextGameRPG.Scripts.GameCore.Services.Battles;

    public static class BotController
    {
        private static readonly SessionManager sessionManager = Services.Get<SessionManager>();
        private static readonly PerformanceManager performanceManager = Services.Get<PerformanceManager>();

        private static bool _isInited;
        private static TelegramBotReceiving _botReceiving;
        private static HttpClient _httpClient = new HttpClient();

        public static string dataPath { get; private set; }
        public static TelegramBotClient botClient { get; private set; }
        public static BotConfig config { get; private set; }
        public static BotDataBase dataBase { get; private set; }

        public static bool isReceiving => _botReceiving != null && _botReceiving.isReceiving;

        public static void Init(string botDataPath)
        {
            if (_isInited)
            {
                Program.logger.Error("BotController is already initialized");
                return;
            }

            dataPath = botDataPath;
            config = GetConfig();
            _botReceiving = new TelegramBotReceiving();

            dataBase = new BotDataBase(botDataPath);
            botClient = new TelegramBotClient(config.token);
            _isInited = true;
        }

        private static BotConfig GetConfig()
        {
            var jsonStr = string.Empty;
            string configPath = Path.Combine(dataPath, "config.json");
            if (!File.Exists(configPath))
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

        public static async Task<bool> StartListening()
        {
            if (isReceiving)
            {
                Program.logger.Error("Bot listening is already started");
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

            await WaitForNetworkConnection().FastAwait();
            SubcribeEvents();
            await Services.OnBotStarted().FastAwait();
            await CharacterStickersHolder.StickersUpdate().FastAwait();
            await _botReceiving.StartReceiving().FastAwait();

            return true;
        }

        public static async Task StopListening()
        {
            if (!isReceiving)
                return;

            _botReceiving.StopReceiving();
            await sessionManager.CloseAllSessions().FastAwait();
            await dataBase.CloseAsync().FastAwait();
            await Services.OnBotStopped().FastAwait();
            UnsubscribeEvents();
        }

        public static async void Reconnect()
        {
            if (!isReceiving)
                return;

            Program.logger.Info("Reconnection starts...");
            _botReceiving.StopReceiving();
            var battleManager = Services.Get<BattleManager>();
            if (battleManager != null)
            {
                var playersInBattle = battleManager.GetAllPlayers();
                battleManager.UnregisterAllBattles(); //специально вызываем перед закрытием сессий!
                foreach (var player in playersInBattle)
                {
                    await sessionManager.CloseSession(player.session.chatId, onError: true);
                }
            }
            await WaitForNetworkConnection().FastAwait();
            await _botReceiving.StartReceiving().FastAwait();
        }

        private static async Task WaitForNetworkConnection()
        {
            while (true)
            {
                Program.logger.Info("Connecting to telegram servers...");
                try
                {
                    using HttpRequestMessage checkConnectionRequest = new HttpRequestMessage(HttpMethod.Get, "https://t.me");
                    var result = await _httpClient.SendAsync(checkConnectionRequest);
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

        private static void SubcribeEvents()
        {
            performanceManager.onStateUpdate += OnUpdatePerformanceState;
        }

        private static void UnsubscribeEvents()
        {
            performanceManager.onStateUpdate -= OnUpdatePerformanceState;
        }

        private static async void OnUpdatePerformanceState(PerformanceState state)
        {
            if (isReceiving && state == PerformanceState.ShutdownRequired)
            {
                Shutdown(fatalError: "Performance limit reached");
            }
        }

        public static async void Shutdown(string? fatalError = null)
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
            
            await StopListening().FastAwait();
            Program.logger.Info("Stopping the server was completed correctly");
            System.Environment.Exit(0);
        }


    }
}
