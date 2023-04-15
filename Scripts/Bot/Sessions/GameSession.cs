using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Profiles;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.Bot.CallbackData;
using TextGameRPG.Scripts.Bot.DataBase.TablesStructure;
using TextGameRPG.Scripts.Bot.Dialogs;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Services;
using System.Threading;
using TextGameRPG.Scripts.GameCore.Services.Battles;

namespace TextGameRPG.Scripts.Bot.Sessions
{
    public class GameSession
    {
        private static readonly SessionManager sessionManager = Services.Get<SessionManager>();
        private static readonly PerformanceManager performanceManager = Services.Get<PerformanceManager>();
        private static readonly BattleManager battleManager = Services.Get<BattleManager>();
        private static readonly MessageSender messageSender = Services.Get<MessageSender>();

        private bool _isHandlingUpdate;
        private CancellationTokenSource _sessionTasksCTS = new CancellationTokenSource();

        public ChatId chatId { get; }
        public ChatId? fakeChatId { get; }
        public DateTime startTime { get; }
        public DateTime lastActivityTime { get; private set; }
        public User actualUser { get; private set; }
        public Profile profile { get; private set; }
        public Player player { get; private set; }
        public LanguageCode language { get; private set; } = BotConfig.instance.defaultLanguageCode;
        public DialogBase? currentDialog { get; private set; }
        public TooltipController tooltipController { get; } = new TooltipController();
        public bool isAdmin => profile.data.adminStatus > 0;

        public GameSession(User user, ChatId? _fakeChatId = null)
        {
            chatId = user.Id;
            fakeChatId = _fakeChatId;
            startTime = DateTime.UtcNow;
            lastActivityTime = DateTime.UtcNow;

            Program.logger.Info($"Started a new session for {user}"
                + (fakeChatId?.Identifier != null ? $" with fakeId: {fakeChatId}" : string.Empty));
            Program.logger.Info($"Sessions Count: {sessionManager.sessionsCount + 1}");
        }

        public void SetupActiveDialog(DialogBase dialog)
        {
            currentDialog?.OnClose();
            currentDialog = dialog;
        }

        public async void HandleUpdateAsync(User refreshedUser, Update update)
        {
            if (_isHandlingUpdate)
            {
                if (update.Type == UpdateType.CallbackQuery)
                {
                    await messageSender.AnswerQuery(refreshedUser.Id, update.CallbackQuery.Id).FastAwait();
                }
                return;
            }

            _isHandlingUpdate = true;
            try
            {
                // Делаем паузу на обработку апдейта в зависимости от нагрузки на процессор
                await Task.Delay(performanceManager.currentResponceDelay).FastAwait();

                lastActivityTime = DateTime.UtcNow;
                actualUser = refreshedUser;
                if (profile == null)
                {
                    await OnStartNewSession(actualUser, update).FastAwait();
                    _isHandlingUpdate = false;
                    return;
                }

                switch (update.Type)
                {
                    case UpdateType.Message:
                        await HandleMessageAsync(update.Message).FastAwait();
                        break;
                    case UpdateType.CallbackQuery:
                        await HandleQueryAsync(update.CallbackQuery).FastAwait();
                        break;
                }
            }
            catch (Exception ex) 
            {
                Program.logger.Error($"Exception in session [user: {refreshedUser}]\n{ex}\n");
                await messageSender.SendErrorMessage(chatId, $"{ex.GetType()}: {ex.Message}").FastAwait();
            }
            _isHandlingUpdate = false;
        }


        public async Task HandleMessageAsync(Message message)
        {
            if (BotConfig.instance.logUserInput && message.Text != null)
            {
                Program.logger.Info($"Message from {actualUser}: {message.Text}");
            }

            if (message.Text != null && message.Text.StartsWith('/'))
            {
                await Commands.CommandHandler.HandleCommand(this, message.Text).FastAwait();
                return;
            }
            await currentDialog.HandleMessage(message).FastAwait();
        }

        public async Task HandleQueryAsync(CallbackQuery query)
        {
            if (query == null || query.Data == null)
                return;

            var callbackData = JsonConvert.DeserializeObject<CallbackDataBase>(query.Data);
            switch (callbackData)
            {
                case DialogPanelButtonCallbackData dialogPanelButtonCallback:
                    if (dialogPanelButtonCallback.sessionTime != startTime.Ticks)
                    {
                        return;
                    }
                    if (currentDialog != null && currentDialog is DialogWithPanel dialogWithPanel)
                    {
                        await dialogWithPanel.HandleCallbackQuery(query.Id, dialogPanelButtonCallback).FastAwait();
                    }
                    return;

                case BattleTooltipCallbackData battleTooltipCallback:
                    if (player == null)
                        return;
                    var currentBattle = battleManager.GetCurrentBattle(player);
                    if (currentBattle == null)
                        return;
                    await currentBattle.HandleBattleTooltipCallback(player, query.Id, battleTooltipCallback).FastAwait();
                    return;
            }
        }

        private async Task OnStartNewSession(User actualUser, Update update)
        {
            var profilesTable = TelegramBot.instance.dataBase[Table.Profiles] as ProfilesDataTable;
            var profileData = await profilesTable.GetOrCreateProfileData(actualUser, fakeChatId).FastAwait();
            if (profileData == null)
            {
                Program.logger.Error($"Can`t get or create profile data after start new session (telegram_id: {actualUser.Id})");
                return;
            }

            var profilesDynamicTable = TelegramBot.instance.dataBase[Table.ProfilesDynamic] as ProfilesDynamicDataTable;
            var profileDynamicData = await profilesDynamicTable.GetOrCreateData(profileData.dbid).FastAwait();
            if (profileDynamicData == null)
            {
                Program.logger.Error($"Can`t get or create profile dynamic data after start new session (telegram_id: {actualUser.Id})");
                return;
            }

            var profileBuildingsTable = TelegramBot.instance.dataBase[Table.ProfileBuildings] as ProfileBuildingsDataTable;
            var profileBuildingsData = await profileBuildingsTable.GetOrCreateData(profileData.dbid).FastAwait();
            if (profileBuildingsData == null)
            {
                Program.logger.Error($"Can`t get or create profile buildings data after start new session (telegram_id: {actualUser.Id})");
                return;
            }

            profile = new Profile(this, profileData, profileDynamicData, profileBuildingsData);
            language = Enum.Parse<LanguageCode>(profileData.language);
            player = new Player(this);

            profile.data.lastDate = DateTime.UtcNow.AsString();
            profile.data.lastVersion = ProjectVersion.Current.ToString();

            await QuestManager.HandleNewSession(this, update).FastAwait();
        }

        public async Task OnCloseSession(bool onError)
        {
            await SaveProfileIfNeed().FastAwait();
            _sessionTasksCTS.Cancel();
            Program.logger.Info($"Session closed for {actualUser}" + (onError ? " [ON ERROR]" : string.Empty));

            if (onError)
            {
                battleManager.OnSessionClosedWithError(player);
            }
        }

        public async Task SaveProfileIfNeed()
        {
            if (profile != null)
            {
                await profile.SaveProfileIfNeed(lastActivityTime).FastAwait();
            }
        }

        public bool IsTasksCancelled()
        {
            if (_sessionTasksCTS.IsCancellationRequested)
                return true;

            return sessionManager.allSessionsTasksCTS.IsCancellationRequested;
        }

        public void SetupLanguage(LanguageCode languageCode)
        {
            language = languageCode;
        }

    }
}
