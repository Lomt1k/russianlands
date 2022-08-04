using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.TelegramBot.CallbackData;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public class BattleTurn
    {
        public const int MOB_TURN_MILISECONDS_DELAY = 7_000;

        private IBattleAction[]? _battleActions;
        private Dictionary<Player, List<BattleTooltipType>> _queryTooltipsToIgnoreByPlayers = new Dictionary<Player, List<BattleTooltipType>>();

        public Battle battle { get; }
        public IBattleUnit unit { get; }
        public IBattleUnit enemy { get; }
        public int secondsLimit { get; }
        public int secondsLeft { get; private set; }
        public bool isLastChance { get; }

        public bool isWaitingForActions => _battleActions == null && secondsLeft > 0;

        public BattleTurn(Battle _battle, IBattleUnit _unit, int _secondsLimit = 60)
        {
            battle = _battle;
            unit = _unit;
            enemy = battle.GetEnemy(unit);
            secondsLimit = _secondsLimit;
            secondsLeft = _secondsLimit;
            isLastChance = unit.unitStats.currentHP <= 0;
        }

        public async Task HandleTurn()
        {
            await enemy.OnStartEnemyTurn(this);

            AddManaPoint();
            AskUnitForBattleActions();
            await WaitAnswersFromAllUnits();
            await InvokeBattleActions();
        }

        private void AddManaPoint()
        {
            unit.unitStats.AddManaPoint();
        }

        private async void AskUnitForBattleActions()
        {
            var selectedActions = await unit.GetActionsForBattleTurn(this);
            if (isWaitingForActions)
            {
                _battleActions = selectedActions.OrderBy(x => x.priority).ToArray();
            }
        }

        private async Task WaitAnswersFromAllUnits()
        {
            while (isWaitingForActions)
            {
                await Task.Delay(1000);
                secondsLeft--;
                if (secondsLeft == 10)
                {
                    unit.OnMineBattleTurnAlmostEnd();
                    continue;
                }
                if (secondsLeft == 0 && _battleActions == null)
                {
                    await unit.OnMineBatteTurnTimeEnd();
                    var enemy = battle.GetEnemy(unit);
                    await enemy.OnEnemyBattleTurnTimeEnd();
                }
            }
        }

        private async Task InvokeBattleActions()
        {
            if (_battleActions == null)
                return;

            var mineStringBuilder = unit is Player ? new StringBuilder() : null;
            var enemyStringBuilder = enemy is Player ? new StringBuilder() : null;

            mineStringBuilder?.AppendLine(Localization.Get(unit.session, "battle_mine_turn_maded"));
            enemyStringBuilder?.AppendLine(Localization.Get(enemy.session, "battle_enemy_turn_maded"));

            foreach (var action in _battleActions)
            {
                action.ApplyActionWithMineStats(unit.unitStats);
                action.ApplyActionWithEnemyStats(enemy.unitStats);

                mineStringBuilder?.AppendLine();
                mineStringBuilder?.AppendLine(action.GetLocalization(unit.session));

                enemyStringBuilder?.AppendLine();
                enemyStringBuilder?.AppendLine(action.GetLocalization(enemy.session));
            }

            var messageSender = TelegramBot.instance.messageSender;
            if (mineStringBuilder != null)
            {
                mineStringBuilder.AppendLine();
                mineStringBuilder.AppendLine(battle.GetStatsView(unit.session));
                var keyboard = BattleToolipHelper.GetStatsKeyboard(unit.session);
                await messageSender.SendTextMessage(unit.session.chatId, mineStringBuilder.ToString(), keyboard);
            }
            if (enemyStringBuilder != null)
            {
                enemyStringBuilder.AppendLine();
                enemyStringBuilder.AppendLine(battle.GetStatsView(enemy.session));
                var keyboard = BattleToolipHelper.GetStatsKeyboard(enemy.session);
                await messageSender.SendTextMessage(enemy.session.chatId, enemyStringBuilder.ToString(), keyboard);
            }
        }

        public async Task HandleBattleTooltipCallback(Player player, string queryId, BattleTooltipCallbackData callback)
        {
            if (!_queryTooltipsToIgnoreByPlayers.ContainsKey(player))
            {
                _queryTooltipsToIgnoreByPlayers[player] = new List<BattleTooltipType>();
            }
            var ingoreList = _queryTooltipsToIgnoreByPlayers[player];
            if (ingoreList.Contains(callback.tooltip))
            {
                await TelegramBot.instance.messageSender.AnswerQuery(queryId);
                return;
            }

            if (callback.tooltip.IsSecondQueryIgnoreRequired())
            {
                ingoreList.Add(callback.tooltip);
            }
            await CreateTooltip(player, queryId, callback);
        }

        private async Task CreateTooltip(Player player, string queryId, BattleTooltipCallbackData callback)
        {
            switch (callback.tooltip)
            {
                case BattleTooltipType.ShowMineStats:
                    await CreateUnitStatsTooltip(player, queryId, unit);
                    break;
                case BattleTooltipType.ShowEnemyStats:
                    await CreateUnitStatsTooltip(player, queryId, battle.GetEnemy(unit));
                    break;
            }
        }

        private async Task CreateUnitStatsTooltip(Player player, string queryId, IBattleUnit targetUnit)
        {
            var messageSender = TelegramBot.instance.messageSender;
            var text = targetUnit.GetFullUnitInfoView(player.session);
            await messageSender.SendTextMessage(player.session.chatId, text);
            await messageSender.AnswerQuery(queryId);
        }



    }
}
