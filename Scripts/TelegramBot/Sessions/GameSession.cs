using Newtonsoft.Json;
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
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.TelegramBot.Managers;

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
        public LanguageCode language { get; private set; } = LanguageCode.en;
        public DialogBase? currentDialog { get; private set; }
        public TooltipController tooltipController { get; } = new TooltipController();
        public bool isAdmin => profile.data.adminStatus > 0;

        private bool _isHandlingUpdate;
        private PerformanceManager _performanceManager;

        public GameSession(User user)
        {
            chatId = user.Id;
            startTime = DateTime.UtcNow;
            lastActivityTime = DateTime.UtcNow;

            _performanceManager = GlobalManagers.performanceManager;
            Program.logger.Info($"Started a new session for {user}");
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
                // Делаем паузу на обработку апдейта в зависимости от нагрузки на процессор
                await Task.Delay(_performanceManager.GetCurrentResponseDelay());

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
                Program.logger.Error($"Exception in session [user: {refreshedUser}]\n{ex}\n");
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
                    if (currentDialog == null)
                        return;
                    await currentDialog.HandleCallbackQuery(query.Id, dialogPanelButtonCallback);
                    return;

                case BattleTooltipCallbackData battleTooltipCallback:
                    if (player == null || Managers.GlobalManagers.battleManager == null)
                        return;
                    var currentBattle = Managers.GlobalManagers.battleManager.GetCurrentBattle(player);
                    if (currentBattle == null)
                        return;
                    await currentBattle.HandleBattleTooltipCallback(player, query.Id, battleTooltipCallback);
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

            var profileBuildingsTable = TelegramBot.instance.dataBase[Table.ProfileBuildings] as ProfileBuildingsDataTable;
            var profileBuildingsData = await profileBuildingsTable.GetOrCreateData(profileData.dbid);
            if (profileBuildingsData == null)
            {
                Program.logger.Error($"Can`t get or create profile buildings data after start new session (telegram_id: {actualUser.Id})");
                return;
            }

            profile = new Profile(this, profileData, profileDynamicData, profileBuildingsData);
            language = Enum.Parse<LanguageCode>(profileData.language);
            player = new Player(this);

            await QuestManager.HandleNewSession(this);
        }

        public async Task OnCloseSession()
        {
            await SaveProfileIfNeed();
            Program.logger.Info($"Session closed for {actualUser}");
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
