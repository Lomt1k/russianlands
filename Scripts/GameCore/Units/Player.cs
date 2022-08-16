using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.CallbackData;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Battle;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.Utils;

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

        public string GetGeneralUnitInfoView(GameSession sessionToSend)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}{Emojis.menuItems[MenuItem.Character]}</b>");
            string levelStr = string.Format(Localization.Get(sessionToSend, "unit_view_level"), session.profile.data.level);
            sb.AppendLine(levelStr);
            return sb.ToString();
        }

        public string GetFullUnitInfoView(GameSession sessionToSend)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}{Emojis.menuItems[MenuItem.Character]}</b>");
            string levelStr = string.Format(Localization.Get(sessionToSend, "unit_view_level"), session.profile.data.level);
            sb.AppendLine(levelStr);
            sb.AppendLine();

            sb.AppendLine(unitStats.GetView(sessionToSend));
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
            sb.AppendLine($"{Emojis.menuItems[MenuItem.Battle]} {Localization.Get(session, "battle_start")} ");
            sb.AppendLine( Localization.Get(session, "battle_your_turn_" + (this == battle.firstUnit ? "first" : "second")) );
            sb.AppendLine();
            sb.AppendLine(battle.GetStatsView(session));

            var keyboard = BattleToolipHelper.GetStatsKeyboard(session);
            await messageSender.SendTextMessage(session.chatId, sb.ToString(), keyboard);
        }

        public async Task<List<IBattleAction>> GetActionsForBattleTurn(BattleTurn battleTurn)
        {
            var result = new List<IBattleAction>();
            IBattleAction? actionBySelection = null;

            var dialog = new SelectBattleActionDialog(session, battleTurn, (selectedAction) => actionBySelection = selectedAction).Start();
            while (actionBySelection == null && battleTurn.isWaitingForActions)
            {
                await Task.Delay(1000);
            }
            if (actionBySelection != null)
            {
                result.Add(actionBySelection);
            }

            //TODO: Add actions from rings and amulets TO result

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

        public bool TryAddShieldOnStartEnemyTurn(out DamageInfo damageInfo)
        {
            damageInfo = DamageInfo.Zero;

            var shield = inventory.equipped[ItemType.Shield];
            if (shield == null)
                return false;

            var blockAbility = shield.data.ablitityByType[AbilityType.BlockIncomingDamageEveryTurn] as BlockIncomingDamageEveryTurnAbility;
            if (blockAbility == null)
                return false;

            var success = Randomizer.TryPercentage(blockAbility.chanceToSuccessPercentage);
            if (!success)
                return false;

            damageInfo = new DamageInfo(
                physicalDamage: blockAbility.physicalDamage,
                fireDamage: blockAbility.fireDamage,
                coldDamage: blockAbility.coldDamage,
                lightningDamage: blockAbility.lightningDamage);
            return true;
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
