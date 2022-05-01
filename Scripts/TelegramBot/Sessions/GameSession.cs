using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Profiles;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.TelegramBot.Sessions
{
    public class GameSession
    {
        public long userId { get; }
        public DateTime startTime { get; }
        public DateTime lastActivityTime { get; private set; }
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
            Program.logger.Info($"Ingame update for logged user " +
                $"(dbid {profile.data.dbid}, telegram_id {profile.data.telegram_id}, username {profile.data.username})");
        }

        private async void OnStartNewSession(User actualUser)
        {
            var profilesTable = TelegramBot.instance.dataBase[Table.Profiles] as ProfilesDataTable;
            var profileData = await profilesTable.GetOrCreateProfileData(actualUser);
            if (profileData == null)
            {
                Program.logger.Error($"Can`t get or create profile data after start new session (telegram_id: {actualUser.Id})");
                return;
            }
            var profilesDynamicTable = TelegramBot.instance.dataBase[Table.ProfilesDynamic] as ProfilesDynamicDataTable;
            var profileDynamicData = await profilesDynamicTable.GetOrCreateData(profileData.dbid);
            if (profileDynamicData == null)
            {
                Program.logger.Error($"Can`t get or create profile dynamic data after start new session (telegram_id: {actualUser.Id})");
                return;
            }
            profile = new Profile(profileData, profileDynamicData);

            //TODO start game
            Program.logger.Info($"Start new game logic... (dbid {profileData.dbid}, telegram_id {profileData.telegram_id}, username {profileData.username})");
        }

        public void OnCloseSession()
        {
            SaveProfileIfNeed();
            Program.logger.Info($"Session closed for ID {userId}");
        }

        public void SaveProfileIfNeed()
        {
            profile?.SaveProfileIfNeed(lastActivityTime);
        }

    }
}
