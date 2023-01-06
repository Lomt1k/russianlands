using Newtonsoft.Json;
using System.Runtime.Serialization;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot
{
    [JsonObject]
    public class TelegramBotConfig
    {
        public string token = "ENTER_TOKEN_HERE";
        public string defaultLanguage = "RU";
        public int sessionTimeoutInMinutes = 180;
        public int periodicSaveDatabaseInMinutes = 15;

        public int cpuUsageLimitInPercents = 80;
        public int cpuUsageToHighloadState = 65;
        public int responceMsDelayWhenCpuHighload = 500;
        public int memoryUsageLimitInMegabytes = 1024;
        public int memoryUsageToHighloadState = 768;
        public int sessionTimeoutInMinutesWhenMemoryHighoad = 30;

        [JsonIgnore]
        public LanguageCode defaultLanguageCode;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (!System.Enum.TryParse(defaultLanguage.ToUpper(), out defaultLanguageCode))
            {
                defaultLanguageCode = LanguageCode.EN;
                Program.logger.Error($"Incorrect language code in config field 'defaultLanguage'. Setuped {defaultLanguageCode} by default");
            }
        }

    }
}
