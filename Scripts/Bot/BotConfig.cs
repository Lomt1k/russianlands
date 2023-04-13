using Newtonsoft.Json;
using System.Runtime.Serialization;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot
{
    [JsonObject]
    public class BotConfig
    {
        [JsonIgnore] public static BotConfig instance { get; private set; }

        public string token = "ENTER_TOKEN_HERE";
        public string defaultLanguage = "RU";
        public bool cheatsForAll = true;
        public bool logUserInput = true;
        public int sessionTimeoutInMinutes = 30;
        public int periodicSaveDatabaseInMinutes = 15;
        public byte sendMessagePerSecondLimit = 25;
        public byte editMessagePerSecondLimit = 30;
        public byte sendStickerPerSecondLimit = 50;
        public int dontSendStickerIfDelayInSeconds = 5;

        public int cpuUsageToHighloadStateInPercents = 80;
        public int responceMsDelayWhenCpuHighload = 500;
        public int appRamUsageLimitInMegabytes = 0;
        public int totalRamUsageLimitInPercents = 95;
        public bool sendTechWorksNotificationsOnStop = true;
        public int secondsLimitForSendTechWorks = 30;

        [JsonIgnore] public LanguageCode defaultLanguageCode { get; private set; }


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (!System.Enum.TryParse(defaultLanguage.ToUpper(), out LanguageCode parsedLanguage))
            {
                parsedLanguage = LanguageCode.EN;
                Program.logger.Error($"Incorrect language code in config field 'defaultLanguage'. Setuped {parsedLanguage} by default");
            }
            defaultLanguageCode = parsedLanguage;
            instance = this;
        }

    }
}
