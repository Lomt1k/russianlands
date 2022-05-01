using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TextGameRPG.Scripts.GameCore.Profiles;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;
using TextGameRPG.Scripts.TelegramBot.Dialogs;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial;

namespace TextGameRPG.Scripts.TelegramBot.Sessions
{
    public class GameSession
    {
        public long userId { get; }
        public DateTime startTime { get; }
        public DateTime lastActivityTime { get; private set; }
        public Profile profile { get; private set; }
        public DialogBase currentDialog { get; private set; }

        public GameSession(User user)
        {
            userId = user.Id;
            startTime = DateTime.UtcNow;
            lastActivityTime = DateTime.UtcNow;
            Program.logger.Info($"Started a new session for @{user.Username} (ID {user.Id})");
        }

        public void SetupActiveDialog(DialogBase dialog)
        {
            currentDialog = dialog;
        }

        public void HandleUpdateAsync(User actualUser, Update update)
        {
            lastActivityTime = DateTime.UtcNow;
            if (profile == null)
            {
                OnStartNewSession(actualUser);
                return;
            }

            switch (update.Type)
            {
                case UpdateType.Message:
                    HandleMessage(actualUser, update.Message);
                    break;
                case UpdateType.CallbackQuery:
                    HandleQuery(actualUser, update.CallbackQuery);
                    break;
            }
        }

        public void HandleMessage(User actualUser, Message message)
        {
            //TODO: add command logic ( /start and etc )
            currentDialog.HandleMessage(actualUser, message);
        }

        public void HandleQuery(User actualUser, CallbackQuery query)
        {
            //TODO
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

            //TODO start session
            Program.logger.Info($"Start new game logic... (dbid {profileData.dbid}, telegram_id {profileData.telegram_id}, username {profileData.username})");
            if (!profileData.isTutorialCompleted)
            {
                TutorialManager.StartCurrentStage(actualUser, profile);
            }
            else
            {
                //TODO
            }
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
