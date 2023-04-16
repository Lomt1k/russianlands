using Newtonsoft.Json;
using System.Runtime.Serialization;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot
{
    [JsonObject]
    public class BotConfig
    {
        public string token = "ENTER_TOKEN_HERE";
        public string defaultLanguage = "RU";
        public bool cheatsForAll = true;
        public int sessionTimeoutInMinutes = 30;
        public int periodicSaveDatabaseInMinutes = 15;

        [JsonIgnore] public LanguageCode defaultLanguageCode { get; private set; }

        public SendingLimits sendingLimits = new SendingLimits();
        public PerformanceSettings performanceSettings = new PerformanceSettings();
        public LogSettings logSettings = new LogSettings();

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
        public class LogSettings
        {
            public bool logUserInput = true;
            public bool logDailyNotifications = true;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (!System.Enum.TryParse(defaultLanguage.ToUpper(), out LanguageCode parsedLanguage))
            {
                parsedLanguage = LanguageCode.EN;
                Program.logger.Error($"Incorrect language code in config field 'defaultLanguage'. Setuped {parsedLanguage} by default");
            }
            defaultLanguageCode = parsedLanguage;
        }

    }

}
