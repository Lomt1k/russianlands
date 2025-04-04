﻿using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Quests.Characters;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Services.BotData;
using MarkOne.Scripts.GameCore.Http;
using MarkOne.Scripts.GameCore.Http.AdminService;
using FastTelegramBot;
using MarkOne.Scripts.GameCore.Http.Payments;

namespace MarkOne.Scripts.Bot;
public static class BotController
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();
    private static readonly PerformanceManager performanceManager = ServiceLocator.Get<PerformanceManager>();

    private static bool _isInited;
    private static TelegramUpdatesReceiver _updatesReceiver;

    public static string dataPath { get; private set; }
    public static string botname { get; private set; }
    public static TelegramBotClient botClient { get; private set; }
    public static BotConfig config { get; private set; }
    public static BotDataBase dataBase { get; private set; }
    public static BotHttpListener httpListener { get; private set; }
    public static HttpClient httpClient => botClient.HttpClient;

    public static bool isReceiving { get; private set; }

    public static void Init(string botDataPath)
    {
        if (_isInited)
        {
            Program.logger.Error("BotController is already initialized");
            return;
        }

        dataPath = botDataPath;
        config = GetConfig();

        dataBase = new BotDataBase(botDataPath);
        botClient = new TelegramBotClient(config.token);
        httpListener = new BotHttpListener(config.httpListenerSettings);
        _updatesReceiver = new TelegramUpdatesReceiver(httpListener);
        _isInited = true;
    }

    private static BotConfig GetConfig()
    {
        var configPath = Path.Combine(dataPath, "config.json");
        string? jsonStr;
        if (!File.Exists(configPath))
        {
            var newConfig = new BotConfig();
            jsonStr = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
            using (var writer = new StreamWriter(configPath, false, Encoding.UTF8))
            {
                writer.Write(jsonStr);
            }
            Program.logger.Warn($"Created new bot config {configPath} \nYou need to enter a token!");
            return newConfig;
        }

        BotConfig loadedConfig;
        using (var reader = new StreamReader(configPath, Encoding.UTF8))
        {
            jsonStr = reader.ReadToEnd();
            loadedConfig = JsonConvert.DeserializeObject<BotConfig>(jsonStr);
        }

        // пересохраняем загруженный конфиг (на случай, если в новой версии приложения были добавлены новые поля - чтобы они появились)
        jsonStr = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
        using (var writer = new StreamWriter(configPath, false, Encoding.UTF8))
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
        bool isConnected = dataBase.Connect();
        if (!isConnected)
        {
            Program.logger.Info($"Reject database connection!...");
            return false;
        }

        await WaitForNetworkConnection().FastAwait();
        SubcribeEvents();
        await ServiceLocator.OnBotStarted().FastAwait();
        await CharacterStickersHolder.StickersUpdate().FastAwait();

        var mineUser = await botClient.GetMeAsync().FastAwait();
        botname = mineUser.Username;
        Program.SetTitle($"{botname} [{dataPath}]");

        
        httpListener.StartListening();
        CreateHttpServices();
        await _updatesReceiver.StartReceiving().FastAwait();
        Program.logger.Info($"Start listening for @{mineUser.Username}");
        isReceiving = true;
        
        return true;
    }

    private static void CreateHttpServices()
    {
        var httpListenerSettings = config.httpListenerSettings;
        var adminServiceSettings = httpListenerSettings.adminServiceSettings;
        if (adminServiceSettings.isEnabled)
        {
            var path = adminServiceSettings.localPath;
            var fullUrl = httpListenerSettings.externalHttpPrefix + path.TrimStart('/');
            httpListener.RegisterHttpService(path, new HttpAdminService(fullUrl, adminServiceSettings));
        }

        var paymentSettings = config.paymentsSettings;
        var webHookPath = paymentSettings.webhookPath;
        if (paymentSettings.paymentProvider == GameCore.Services.Payments.PaymentProviderType.LAVA_RU)
        {
            httpListener.RegisterHttpService(webHookPath, new LavaPaymentsHttpWebhookService());
        }
    }

    public static async Task StopListening()
    {
        if (!isReceiving)
            return;

        _updatesReceiver.StopReceiving();
        httpListener.StopListening();
        await sessionManager.CloseAllSessions().FastAwait();
        dataBase.Close();
        await ServiceLocator.OnBotStopped().FastAwait();
        UnsubscribeEvents();
        Program.logger.Info($"Listening has been stopped");
        isReceiving = false;
    }

    public static async void Reconnect()
    {
        if (!isReceiving)
            return;

        isReceiving = false;
        {
            Program.logger.Info("Reconnection starts...");
            _updatesReceiver.StopReceiving();
            httpListener.StopListening();
            var battleManager = ServiceLocator.Get<BattleManager>();
            var playersInBattle = battleManager.GetAllPlayers();
            battleManager.UnregisterAllBattles(); //специально вызываем перед закрытием сессий!
            foreach (var player in playersInBattle)
            {
                await sessionManager.CloseSession(player.session.chatId, onError: true);
            }

            await WaitForNetworkConnection().FastAwait();
            httpListener.StopListening();
            _updatesReceiver.StartReceiving();
        }
        isReceiving = true;
    }

    private static async Task WaitForNetworkConnection()
    {
        while (true)
        {
            Program.logger.Info("Connecting to telegram servers...");
            try
            {
                using var checkConnectionRequest = new HttpRequestMessage(HttpMethod.Get, "https://t.me");
                var result = await httpClient.SendAsync(checkConnectionRequest);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Program.logger.Info($"Connected to telegram servers");
                    return; //success!
                }
            }
            catch (System.Exception)
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
        await Task.Delay(500); //for correct logs
        System.Environment.Exit(0);
    }


}
