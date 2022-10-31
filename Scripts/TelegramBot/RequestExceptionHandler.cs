using Telegram.Bot.Exceptions;

namespace TextGameRPG.Scripts.TelegramBot
{
    public class RequestExceptionHandler
    {
        public void HandleException(RequestException ex)
        {
            Program.logger.Error($"Catched exception on sending request...");
            if (ex.InnerException != null)
            {
                Program.logger.Error($"{ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
            }
            else
            {
                Program.logger.Error($"{ex.GetType().Name}: {ex.Message}");
            }
            TelegramBot.instance.Restart();
        }

    }
}
