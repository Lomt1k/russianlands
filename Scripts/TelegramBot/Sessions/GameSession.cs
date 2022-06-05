using Newtonsoft.Json;
using JsonKnownTypes;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Profiles;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.TelegramBot.CallbackData;
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

        private bool _isHandlingUpdate;

        public GameSession(User user)
        {
            chatId = user.Id;
            startTime = DateTime.UtcNow;
            lastActivityTime = DateTime.UtcNow;
            Program.logger.Info($"Started a new session for @{user.Username} (ID {user.Id})");
        }

        public void SetupActiveDialog(DialogBase dialog)
        {
            currentDialog?.OnClose();
            currentDialog = dialog;
        }

        public async Task HandleUpdateAsync(User refreshedUser, Update update)
        {
            if (_isHandlingUpdate)
            {
                if (update.Type == UpdateType.CallbackQuery)
                {
                    await TelegramBot.instance.messageSender.AnswerQuery(update.CallbackQuery.Id);
                }
                return;
            }

            _isHandlingUpdate = true;
            try
            {
                lastActivityTime = DateTime.UtcNow;
                actualUser = refreshedUser;
                if (profile == null)
                {
                    await OnStartNewSession(actualUser);
                    _isHandlingUpdate = false;
                    return;
                }

                switch (update.Type)
                {
                    case UpdateType.Message:
                        await HandleMessageAsync(update.Message);
                        break;
                    case UpdateType.CallbackQuery:
                        await HandleQueryAsync(update.CallbackQuery);
                        break;
                }
            }
            catch (Exception ex) 
            {
                Program.logger.Error($"Exception in session [username @{actualUser.Username}, userId {actualUser.Id}]\n{ex}\n");
                await TelegramBot.instance.messageSender.SendErrorMessage(chatId, $"{ex.GetType()}: {ex.Message}");
            }
            _isHandlingUpdate = false;
        }


        public async Task HandleMessageAsync(Message message)
        {
            if (message.Text != null && message.Text.StartsWith('/'))
            {
                await Commands.CommandHandler.HandleCommand(this, message.Text);
                return;
            }
            await currentDialog.HandleMessage(message);
        }

        public async Task HandleQueryAsync(CallbackQuery query)
        {
            if (query == null || query.Data == null)
                return;

            var callbackData = JsonConvert.DeserializeObject<CallbackDataBase>(query.Data);
            switch (callbackData)
            {
                case DialogPanelButtonCallbackData dialogPanelButtonCallback:
                    await currentDialog.HandleCallbackQuery(query.Id, dialogPanelButtonCallback);
                    return;
            }
        }

        private async Task OnStartNewSession(User actualUser)
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
                await TutorialManager.StartCurrentStage(this);
            }
            else
            {
                await new TownEntryDialog(this, TownEntryReason.StartNewSession).Start();
            }
        }

        public async Task OnCloseSession()
        {
            await SaveProfileIfNeed();
            Program.logger.Info($"Session closed for ID {chatId}");
        }

        public async Task SaveProfileIfNeed()
        {
            if (profile != null)
            {
                await profile.SaveProfileIfNeed(lastActivityTime);
            }
        }

        public void SetupLanguage(LanguageCode languageCode)
        {
            language = languageCode;
        }

    }
}
