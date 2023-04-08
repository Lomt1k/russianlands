using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.Bot.CallbackData;
using TextGameRPG.Scripts.GameCore.Managers.Battles.Actions;
using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Managers.Battles
{
    public class BattleTurn
    {
        public const int MOB_TURN_MILISECONDS_DELAY = 3_000;

        private List<IBattleAction>? _battleActions = null;
        private Dictionary<Player, List<BattleTooltipType>> _queryTooltipsToIgnoreByPlayers = new Dictionary<Player, List<BattleTooltipType>>();

        public Action? onBattleTurnEnd;

        public Battle battle { get; }
        public IBattleUnit unit { get; }
        public IBattleUnit enemy { get; }
        public int millisecondsLeft { get; private set; }
        public bool isLastChance { get; }

        public bool isWaitingForActions => _battleActions == null && millisecondsLeft > 0 && !battle.IsCancellationRequested();

        public BattleTurn(Battle _battle, IBattleUnit _unit, int _secondsLimit)
        {
            battle = _battle;
            unit = _unit;
            enemy = battle.GetEnemy(unit);
            millisecondsLeft = _secondsLimit * 1_000;
            isLastChance = unit.unitStats.currentHP <= 0;
        }

        public async Task HandleTurn()
        {
            if (battle.IsCancellationRequested())
                return;

            if (unit.unitStats.isSkipNextTurnRequired)
            {
                unit.unitStats.isSkipNextTurnRequired = false;
                return;
            }

            await enemy.OnStartEnemyTurn(this).FastAwait();
            OnStartMineTurn();
            AskUnitForBattleActions();
            await WaitAnswerFromUnit().FastAwait();
            await InvokeBattleActions().FastAwait();
            OnEndMineTurn();
        }

        private void OnStartMineTurn()
        {
            unit.unitStats.OnStartMineTurn();
        }

        private async void AskUnitForBattleActions()
        {
            var attackActions = await unit.actionHandler.GetActionsBySelectedItem(this).FastAwait();
            if (!isWaitingForActions)
                return;

            var actionsList = new List<IBattleAction>();
            actionsList.AddRange(attackActions);
            TryAppendEveryTurnActions(actionsList);
            // Важно, что мы присваиваем уже полностью готовый список, который больше не модифицируем
            _battleActions = actionsList;
        }

        private void TryAppendEveryTurnActions(List<IBattleAction> actionsList)
        {
            // --- блок щитом / мечом
            if (enemy.actionHandler.TryAddShieldOnEnemyTurn(out DamageInfo enemyShieldBlock))
            {
                var enemyShieldAction = new AddShieldOnEnemyTurnAction(this, enemyShieldBlock);
                actionsList.Insert(0, enemyShieldAction);
            }
            else if (enemy.actionHandler.TryAddSwordBlockOnEnemyTurn(out DamageInfo enemySwordBlock))
            {
                var enemySwordAction = new AddSwordBlockOnEnemyTurnAction(this, enemySwordBlock);
                actionsList.Insert(0, enemySwordAction);
            }

            var everyTurnActions = unit.actionHandler.GetEveryTurnActions(this);
            actionsList.AddRange(everyTurnActions);
        }

        private async Task WaitAnswerFromUnit()
        {
            while (isWaitingForActions)
            {
                await Task.Delay(500).FastAwait();
                millisecondsLeft -= 500;
                if (millisecondsLeft == 20_000)
                {
                    unit.OnMineBattleTurnAlmostEnd();
                    continue;
                }
                if (millisecondsLeft == 0 && _battleActions == null)
                {
                    await unit.OnMineBatteTurnTimeEnd().FastAwait();
                    var enemy = battle.GetEnemy(unit);
                    await enemy.OnEnemyBattleTurnTimeEnd().FastAwait();
                }
            }
        }

        private async Task InvokeBattleActions()
        {
            if (battle.IsCancellationRequested())
                return;
            if (_battleActions == null)
                return;

            var mineStringBuilder = unit is Player ? new StringBuilder() : null;
            var enemyStringBuilder = enemy is Player ? new StringBuilder() : null;

            mineStringBuilder?.AppendLine(Localization.Get(unit.session, "battle_mine_turn_maded"));
            enemyStringBuilder?.AppendLine(Localization.Get(enemy.session, "battle_enemy_turn_maded", unit.nickname));

            foreach (var action in _battleActions)
            {
                action.ApplyActionWithMineStats(unit.unitStats);
                action.ApplyActionWithEnemyStats(enemy.unitStats);

                mineStringBuilder?.AppendLine();
                mineStringBuilder?.AppendLine(action.GetHeader(unit.session));
                mineStringBuilder?.AppendLine(action.GetDescription(unit.session));

                enemyStringBuilder?.AppendLine();
                enemyStringBuilder?.AppendLine(action.GetHeader(enemy.session));
                enemyStringBuilder?.AppendLine(action.GetDescription(enemy.session));
            }

            var messageSender = TelegramBot.instance.messageSender;
            if (mineStringBuilder != null)
            {
                mineStringBuilder.AppendLine();
                mineStringBuilder.AppendLine(battle.GetStatsView(unit.session));
                var keyboard = BattleToolipHelper.GetStatsKeyboard(unit.session);
                await messageSender.SendTextMessage(unit.session.chatId, mineStringBuilder.ToString(), keyboard).FastAwait();
            }
            if (enemyStringBuilder != null)
            {
                enemyStringBuilder.AppendLine();
                enemyStringBuilder.AppendLine(battle.GetStatsView(enemy.session));
                var keyboard = BattleToolipHelper.GetStatsKeyboard(enemy.session);
                await messageSender.SendTextMessage(enemy.session.chatId, enemyStringBuilder.ToString(), keyboard).FastAwait();
            }
        }

        private void OnEndMineTurn()
        {
            onBattleTurnEnd?.Invoke();
            onBattleTurnEnd = null;
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
                await TelegramBot.instance.messageSender.AnswerQuery(player.session.chatId, queryId).FastAwait();
                return;
            }

            if (callback.tooltip.IsSecondQueryIgnoreRequired())
            {
                ingoreList.Add(callback.tooltip);
            }
            await CreateTooltip(player, queryId, callback).FastAwait();
        }

        private async Task CreateTooltip(Player player, string queryId, BattleTooltipCallbackData callback)
        {
            switch (callback.tooltip)
            {
                case BattleTooltipType.ShowMineStats:
                    await CreateUnitStatsTooltip(player, queryId, unit).FastAwait();
                    break;
                case BattleTooltipType.ShowEnemyStats:
                    await CreateUnitStatsTooltip(player, queryId, battle.GetEnemy(unit)).FastAwait();
                    break;
            }
        }

        private async Task CreateUnitStatsTooltip(Player player, string queryId, IBattleUnit targetUnit)
        {
            var messageSender = TelegramBot.instance.messageSender;
            var text = targetUnit.GetFullUnitInfoView(player.session);
            await messageSender.SendTextMessage(player.session.chatId, text).FastAwait();
            await messageSender.AnswerQuery(player.session.chatId, queryId).FastAwait();
        }



    }
}
