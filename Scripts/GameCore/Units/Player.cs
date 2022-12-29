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
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Units.ActionHandlers;
using TextGameRPG.Scripts.GameCore.Potions;
using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class Player : IBattleUnit
    {
        public GameSession session { get; }
        public UnitStats unitStats { get; }
        public IBattleActionHandler actionHandler { get; }
        public PlayerResources resources { get; }
        public PlayerBuildings buildings { get; }
        public HealthRegenerationController healhRegenerationController { get; }
        public PlayerInventory inventory => session.profile.dynamicData.inventory;
        public List<PotionItem> potions => session.profile.dynamicData.potions;
        public string nickname => session.profile.data.nickname;
        public byte level => session.profile.data.level;

        private static MessageSender messageSender => Bot.TelegramBot.instance.messageSender;

        public Player(GameSession _session)
        {
            session = _session;
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
            sb.AppendLine($"{Emojis.characters[CharIcon.Male]} <b>{nickname}</b>"
                + (isPremium ? Emojis.space + Emojis.stats[Stat.Premium] : string.Empty));
            string levelStr = string.Format(Localization.Get(sessionToSend, "unit_view_level"), level);
            sb.AppendLine(levelStr);
            return sb.ToString();
        }

        public string GetFullUnitInfoView(GameSession sessionToSend)
        {
            var sb = new StringBuilder();
            sb.Append(GetGeneralUnitInfoView(sessionToSend));

            sb.AppendLine();
            sb.AppendLine(unitStats.GetView(sessionToSend));
            return sb.ToString();
        }

        public async Task OnStartBattle(Battle battle)
        {
            unitStats.OnStartBattle();

            var sb = new StringBuilder();
            sb.AppendLine($"{Emojis.menuItems[MenuItem.Battle]} {Localization.Get(session, "battle_start")} ");
            sb.AppendLine( Localization.Get(session, "battle_your_turn_" + (this == battle.firstUnit ? "first" : "second")) );
            sb.AppendLine();
            sb.AppendLine(battle.GetStatsView(session));

            var keyboard = BattleToolipHelper.GetStatsKeyboard(session);
            await messageSender.SendTextMessage(session.chatId, sb.ToString(), keyboard);
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
                sb.AppendLine($"{Emojis.elements[Element.BrokenHeart]} {Localization.Get(session, "battle_enemy_turn_start_last_chance")}");
                sb.AppendLine();
            }

            var waitingText = $"{Emojis.elements[Element.Hourgrlass]} {Localization.Get(session, "battle_enemy_turn_start")}";
            sb.AppendLine(waitingText);
            var keyboard = new ReplyKeyboardMarkup(waitingText);
            await messageSender.SendTextDialog(session.chatId, sb.ToString(), keyboard, silent: true);
        }

        public async void OnMineBattleTurnAlmostEnd()
        {
            var text = $"{Emojis.elements[Element.WarningGrey]} {Localization.Get(session, "battle_mine_turn_almost_end")}";
            await messageSender.SendTextMessage(session.chatId, text, silent: true);
        }

        public async Task OnMineBatteTurnTimeEnd()
        {
            var text = $"{Localization.Get(session, "battle_mine_turn_time_end")} {Emojis.smiles[Smile.Sad]}";
            await messageSender.SendTextMessage(session.chatId, text, silent: true);
        }

        public async Task OnEnemyBattleTurnTimeEnd()
        {
            var text = Localization.Get(session, "battle_enemy_turn_time_end");
            await messageSender.SendTextMessage(session.chatId, text, silent: true);
        }

    }
}
