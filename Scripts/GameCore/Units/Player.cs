using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.CallbackData;
using TextGameRPG.Scripts.Bot.Dialogs.Battle;
using TextGameRPG.Scripts.GameCore.Services.Battles;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Units.ActionHandlers;
using TextGameRPG.Scripts.GameCore.Potions;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Skills;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class Player : IBattleUnit
    {
        private static readonly MessageSender messageSender = Services.Services.Get<MessageSender>();

        public GameSession session { get; }
        public UnitStats unitStats { get; }
        public IBattleActionHandler actionHandler { get; }
        public PlayerResources resources { get; }
        public PlayerBuildings buildings { get; }
        public PlayerSkills skills { get; }
        public HealthRegenerationController healhRegenerationController { get; }
        public PlayerInventory inventory => session.profile.dynamicData.inventory;
        public List<PotionItem> potions => session.profile.dynamicData.potions;
        public string nickname => session.profile.data.nickname;
        public byte level => session.profile.data.level;

        public Player(GameSession _session)
        {
            session = _session;
            skills = new PlayerSkills(this); // before unitStats!
            unitStats = new PlayerStats(this);
            actionHandler = new PlayerActionHandler(this);
            resources = new PlayerResources(_session);
            buildings = new PlayerBuildings(_session);

            healhRegenerationController = new HealthRegenerationController(this);
        }

        public string GetGeneralUnitInfoView(GameSession sessionToSend)
        {
            var sb = new StringBuilder();
            bool isPremium = session.profile.data.IsPremiumActive();
            sb.AppendLine(Emojis.AvatarMale + nickname.Bold() + (isPremium ? Emojis.StatPremium : Emojis.Empty));
            sb.AppendLine(Localization.Get(sessionToSend, "unit_view_level", level));
            return sb.ToString();
        }

        public string GetFullUnitInfoView(GameSession sessionToSend, bool withHealth = true)
        {
            var sb = new StringBuilder();
            sb.Append(GetGeneralUnitInfoView(sessionToSend));

            sb.AppendLine();
            sb.AppendLine(unitStats.GetView(sessionToSend, withHealth));

            if (IsSkillsAvailable())
            {
                sb.AppendLine();
                sb.AppendLine(skills.GetShortView());
            }

            return sb.ToString();
        }
        
        public bool IsSkillsAvailable()
        {
            return buildings.HasBuilding(BuildingType.ElixirWorkshop);
        }

        public async Task OnStartBattle(Battle battle)
        {
            unitStats.OnStartBattle();

            var sb = new StringBuilder();
            sb.AppendLine(Emojis.ButtonBattle + Localization.Get(session, "battle_start"));
            sb.AppendLine( Localization.Get(session, "battle_your_turn_" + (this == battle.firstUnit ? "first" : "second")) );
            sb.AppendLine();
            sb.AppendLine(battle.GetStatsView(session));

            var keyboard = BattleToolipHelper.GetStatsKeyboard(session);
            await messageSender.SendTextMessage(session.chatId, sb.ToString(), keyboard, cancellationToken: session.cancellationToken).FastAwait();
        }

        public void OnBattleEnd(Battle battle, BattleResult battleResult)
        {
            unitStats.OnBattleEnd();
            healhRegenerationController.SetLastRegenTimeAsNow();
        }

        public async Task OnStartEnemyTurn(BattleTurn battleTurn)
        {
            var sb = new StringBuilder();
            if (battleTurn.isLastChance)
            {
                sb.AppendLine(Emojis.ElementBrokenHeart + Localization.Get(session, "battle_enemy_turn_start_last_chance"));
                sb.AppendLine();
            }

            var waitingText = Emojis.ElementHourgrlass + Localization.Get(session, "battle_enemy_turn_start");
            sb.AppendLine(waitingText);
            var keyboard = new ReplyKeyboardMarkup(waitingText);
            await messageSender.SendTextDialog(session.chatId, sb.ToString(), keyboard, silent: true, cancellationToken: session.cancellationToken).FastAwait();
        }

        public async void OnMineBattleTurnAlmostEnd()
        {
            var text = Emojis.ElementWarningGrey + Localization.Get(session, "battle_mine_turn_almost_end");
            await messageSender.SendTextMessage(session.chatId, text, silent: true, cancellationToken: session.cancellationToken).FastAwait();
        }

        public async Task OnMineBatteTurnTimeEnd()
        {
            var text = Localization.Get(session, "battle_mine_turn_time_end") + Emojis.SmileSad;
            await messageSender.SendTextMessage(session.chatId, text, silent: true, cancellationToken: session.cancellationToken).FastAwait();
        }

        public async Task OnEnemyBattleTurnTimeEnd()
        {
            var text = Localization.Get(session, "battle_enemy_turn_time_end");
            await messageSender.SendTextMessage(session.chatId, text, silent: true, cancellationToken: session.cancellationToken).FastAwait();
        }

    }
}
