using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.GameCore.Profiles;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;
using TextGameRPG.Scripts.TelegramBot.Dialogs;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Town;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial;

namespace TextGameRPG.Scripts.TelegramBot.Sessions
{
    public class GameSession
    {
        public ChatId chatId { get; }
        public DateTime startTime { get; }
        public DateTime lastActivityTime { get; private set; }
        public User actualUser { get; private set; }
        public Profile profile { get; private set; }
        public Player player { get; private set; }
        public LanguageCode language { get; private set; }
        public DialogBase currentDialog { get; private set; }

        public GameSession(User user)
        {
            chatId = user.Id;
            startTime = DateTime.UtcNow;
            lastActivityTime = DateTime.UtcNow;
            Program.logger.Info($"Started a new session for @{user.Username} (ID {user.Id})");
        }

        public void SetupActiveDialog(DialogBase dialog)
        {
            currentDialog = dialog;
        }

        public void HandleUpdateAsync(User refreshedUser, Update update)
        {
            lastActivityTime = DateTime.UtcNow;
            actualUser = refreshedUser;
            if (profile == null)
            {
                OnStartNewSession(actualUser);
                return;
            }

            switch (update.Type)
            {
                case UpdateType.Message:
                    HandleMessage(update.Message);
                    break;
                case UpdateType.CallbackQuery:
                    HandleQuery(update.CallbackQuery);
                    break;
            }
        }

        public void HandleMessage(Message message)
        {
            //TODO: add command logic ( /start and etc )
            currentDialog.HandleMessage(message);
        }

        public void HandleQuery(CallbackQuery query)
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
            language = Enum.Parse<LanguageCode>(profileData.language);
            player = new Player(this);

            if (!profileData.isTutorialCompleted)
            {
                TutorialManager.StartCurrentStage(this);
            }
            else
            {
                new TownEntryDialog(this, TownEntryReason.StartNewSession).Start();
            }
        }

        public async Task OnCloseSession()
        {
            await SaveProfileIfNeed();
            Program.logger.Info($"Session closed for ID {chatId}");
        }

        public async Task SaveProfileIfNeed()
        {
            await profile?.SaveProfileIfNeed(lastActivityTime);
        }

        public void SetupLanguage(LanguageCode languageCode)
        {
            language = languageCode;
        }

    }
}
