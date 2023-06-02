using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MarkOne.Scripts.GameCore.Localizations;

namespace MarkOne.Scripts.Bot;

[JsonObject]
public class BotConfig
{
    public string token = "ENTER_TOKEN_HERE";
    public string[] languages = { "RU" };
    public bool cheatsForAll = true;
    public int sessionTimeoutInMinutes = 30;
    public int periodicSaveDatabaseInMinutes = 15;

    public SendingLimits sendingLimits = new SendingLimits();
    public PerformanceSettings performanceSettings = new PerformanceSettings();
    public HttpListenerSettings httpListenerSettings = new HttpListenerSettings();
    public LogSettings logSettings = new LogSettings();

    [JsonIgnore] public LanguageCode[] languageCodes { get; private set; } = { LanguageCode.RU };
    [JsonIgnore] public LanguageCode defaultLanguageCode { get; private set; }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        ParseLanguageCodes();
    }

    [JsonObject]
    public class SendingLimits
    {
        public byte sendMessagePerSecondLimit = 25;
        public byte editMessagePerSecondLimit = 30;
        public byte sendStickerPerSecondLimit = 50;
        public int dontSendStickerIfDelayInSeconds = 5;
    }

    [JsonObject]
    public class PerformanceSettings
    {
        public int cpuUsageToHighloadStateInPercents = 80;
        public int responceMsDelayWhenCpuHighload = 500;
        public int appRamUsageLimitInMegabytes = 0;
        public int totalRamUsageLimitInPercents = 95;
        public bool sendMaintenanceNotificationsOnStop = true;
        public int secondsLimitForSendMaintenance = 30;
    }

    [JsonObject]
    public class HttpListenerSettings
    {
        public string httpPrefix = "http://localhost:1111/";
        public string externalHttpPrefix = "https://localhost/";
        public byte maxConnections = 50;
        public TelegramWebhookSettings telegramWebhookSettings = new();
    }

    [JsonObject]
    public class TelegramWebhookSettings
    {
        public bool useWebhook = false;
        public byte maxConnections = 40;
    }

    [JsonObject]
    public class LogSettings
    {
        public bool logUserInput = true;
        public bool logDailyNotifications = true;
    }

    private void ParseLanguageCodes()
    {
        var allNames = Enum.GetNames(typeof(LanguageCode));
        var sb = new StringBuilder();
        foreach (var name in allNames)
        {
            sb.Append(name + " ");
        }

        var languagesSet = new HashSet<LanguageCode>();
        foreach (var language in languages)
        {
            if (!Enum.TryParse(language.ToUpper(), out LanguageCode parsedLanguage))
            {
                Program.logger.Fatal($"CONFIG ERROR: Unknown language code '{language}' in field 'languages'.\nAvailable languages: {sb}");
                Environment.Exit(1);
                return;
            }
            languagesSet.Add(parsedLanguage);
        }
        if (languagesSet.Count < 1)
        {
            Program.logger.Fatal($"CONFIG ERROR: Field 'languages' is empty.\nPlease add any language: {sb}");
            Environment.Exit(1);
            return;
        }

        languageCodes = languagesSet.ToArray();
        defaultLanguageCode = languageCodes[0];
    }

}
