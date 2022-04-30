using System;
using Telegram.Bot.Types;

namespace TextGameRPG.Scripts.TelegramBot.Sessions
{
    public class GameSession
    {
        public long userId { get; }
        public DateTime startTime { get; }
        public DateTime lastActivityTime { get; private set; }

        public GameSession(User user)
        {
            userId = user.Id;
            startTime = DateTime.UtcNow;
            lastActivityTime = DateTime.UtcNow;
            Program.logger.Info($"Started a new session for @{user.Username} (ID {user.Id})");
        }

        public void HandleUpdateAsync(User actualUser, Update update)
        {
            lastActivityTime = DateTime.UtcNow;
        }

        public void OnCloseSession()
        {
            Program.logger.Info($"Session closed for ID {userId}");
        }

    }
}
