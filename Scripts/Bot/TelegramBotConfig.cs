
namespace TextGameRPG.Scripts.Bot
{
    public class TelegramBotConfig
    {
        public string token = "ENTER_TOKEN_HERE";
        public int sessionTimeoutInMinutes = 180;
        public int periodicSaveDatabaseInMinutes = 15;

        public int cpuUsageLimitInPercents = 80;
        public int cpuUsageToHighloadState = 65;
        public int responceMsDelayWhenCpuHighload = 500;
        public int memoryUsageLimitInMegabytes = 1024;
        public int memoryUsageToHighloadState = 768;
        public int sessionTimeoutInMinutesWhenMemoryHighoad = 30;
    }
}
