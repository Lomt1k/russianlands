using System;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.TelegramBot.DataBase;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.TelegramBot.Sessions
{
    public class GameSession
    {
        public long userId { get; }
        public DateTime startTime { get; }
        public DateTime lastActivityTime { get; private set; }
        public DateTime lastSaveProfileTime { get; private set; }
        public Profile profile { get; private set; }

        public GameSession(User user)
        {
            userId = user.Id;
            startTime = DateTime.UtcNow;
            lastActivityTime = DateTime.UtcNow;
            Program.logger.Info($"Started a new session for @{user.Username} (ID {user.Id})");
        }

        public async void HandleUpdateAsync(User actualUser, Update update)
        {
            lastActivityTime = DateTime.UtcNow;
            if (profile == null)
            {
                OnStartNewSession(actualUser);
                return;
            }

            //TODO
            Program.logger.Info($"Ingame update for logged user (dbid {profile.dbid}, telegram_id {profile.telegram_id}, username {profile.username})");
        }

        private async void OnStartNewSession(User actualUser)
        {
            var profilesTable = TelegramBot.instance.dataBase[Table.Profiles] as ProfilesTable;
            profile = await profilesTable.GetOrCreateProfile(actualUser);
            if (profile == null)
            {
                Program.logger.Error($"Can`t get or create profile after start new session (telegram_id: {actualUser.Id})");
                return;
            }

            //TODO start game
            Program.logger.Info($"Start new game logic... (dbid {profile.dbid}, telegram_id {profile.telegram_id}, username {profile.username})");
        }

        public void OnCloseSession()
        {
            SaveProfileIfNeed();
            Program.logger.Info($"Session closed for ID {userId}");
        }

        public void SaveProfileIfNeed()
        {
            if (profile != null && lastSaveProfileTime < lastActivityTime)
            {
                profile.UpdateInDatabase();
                lastSaveProfileTime = DateTime.UtcNow;
            }
        }

    }
}
