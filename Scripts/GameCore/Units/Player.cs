using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Battle;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class Player : IBattleUnit
    {
        public GameSession session { get; private set; }
        public UnitStats unitStats { get; private set; }
        public PlayerResources resources { get; private set; }
        public PlayerInventory inventory => session.profile.dynamicData.inventory;
        public string nickname => session.profile.data.nickname;

        private static MessageSender messageSender => TelegramBot.TelegramBot.instance.messageSender;

        public Player(GameSession _session)
        {
            session = _session;
            unitStats = new PlayerStats(this);
            resources = new PlayerResources(_session);
        }

        public string GetGeneralInfoView()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}{Emojis.menuItems[MenuItem.Character]}</b>");
            string levelStr = string.Format(Localization.Get(session, "unit_view_level"), session.profile.data.level);
            sb.AppendLine(levelStr);
            return sb.ToString();
        }

        public string GetFullUnitView(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}{Emojis.menuItems[MenuItem.Character]}</b>");
            string levelStr = string.Format(Localization.Get(session, "unit_view_level"), session.profile.data.level);
            sb.AppendLine(levelStr);

            sb.AppendLine(unitStats.GetView(session));
            return sb.ToString();
        }

        public string GetStartTurnView(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}{Emojis.menuItems[MenuItem.Character]}</b>");

            sb.AppendLine(unitStats.GetView(session));
            return sb.ToString();
        }

        public async void OnStartBattle(Battle battle)
        {
            unitStats.OnStartBattle();

            var sb = new StringBuilder();
            var header = $"{Emojis.menuItems[MenuItem.Battle]} "
                + string.Format(Localization.Get(session, "battle_start"), battle.firstUnit.nickname, battle.secondUnit.nickname);
            sb.AppendLine(header);
            sb.AppendLine(Localization.Get(session, "battle_your_turn_" + (this == battle.firstUnit ? "first" : "second") ));
            sb.AppendLine();
            sb.AppendLine(battle.GetStatsView(session));

            await messageSender.SendTextMessage(session.chatId, sb.ToString());
        }

        public async Task<List<IBattleAction>> GetActionsForBattleTurn(BattleTurn battleTurn)
        {
            var result = new List<IBattleAction>();

            var dialog = new SelectBattleActionDialog(session, battleTurn, (selectedActions) => result = selectedActions).Start();
            while (result.Count == 0 && battleTurn.isWaitingForActions)
            {
                await Task.Delay(1000);
            }

            return result;
        }

        public async Task OnStartEnemyTurn(BattleTurn battleTurn)
        {
            var sb = new StringBuilder();
            if (battleTurn.isLastChance)
            {
                sb.AppendLine($"{Emojis.elements[Element.BrokenHeart]} {Localization.Get(session, "battle_enemy_turn_start_last_chance")}");
                sb.AppendLine();
            }

            sb.AppendLine($"{Emojis.elements[Element.Hourgrlass]} {Localization.Get(session, "battle_enemy_turn_start")}");
            await messageSender.SendTextMessage(session.chatId, sb.ToString(), silent: true);
        }

        public async void OnMineBattleTurnAlmostEnd()
        {
            var text = $"{Emojis.elements[Element.WarningGrey]} {Localization.Get(session, "battle_mine_turn_almost_end")}";
            await messageSender.SendTextMessage(session.chatId, text, silent: true);
        }

        public async Task OnMineBatteTurnTimeEnd()
        {
            var text = $"{Localization.Get(session, "battle_mine_turn_time_end")} {Emojis.smiles[Smile.Sad]}";
            await messageSender.SendTextDialog(session.chatId, text, silent: true);
        }

        public async Task OnEnemyBattleTurnTimeEnd()
        {
            var text = Localization.Get(session, "battle_enemy_turn_time_end");
            await messageSender.SendTextDialog(session.chatId, text, silent: true);
        }
    }
}
