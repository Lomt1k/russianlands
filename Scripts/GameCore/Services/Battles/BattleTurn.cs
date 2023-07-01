using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.CallbackData;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.Battles.Actions;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Services.Battles;

public class BattleTurn
{
    public const int MOB_TURN_MILISECONDS_DELAY = 3_000;

    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    private List<IBattleAction>? _battleActions = null;
    private readonly Dictionary<Player, HashSet<BattleTooltipType>> _queryTooltipsToIgnoreByPlayers = new();

    public Action? onBattleTurnEnd;

    public Battle battle { get; }
    public IBattleUnit unit { get; }
    public IBattleUnit enemy { get; }
    public int millisecondsLeft { get; private set; }
    public bool isLastChance { get; }
    public bool isFirstUnit { get; }
    public int turnNumber { get; }

    public bool isWaitingForActions => _battleActions == null && millisecondsLeft > 0 && !battle.IsCancellationRequested();

    public BattleTurn(Battle _battle, IBattleUnit _unit, int _secondsLimit, bool _isFirstUnit, int _turnNumber)
    {
        battle = _battle;
        unit = _unit;
        enemy = battle.GetEnemy(unit);
        millisecondsLeft = _secondsLimit * 1_000;
        isLastChance = unit.unitStats.currentHP <= 0;
        isFirstUnit = _isFirstUnit;
        turnNumber = _turnNumber;
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
        if (enemy.actionHandler.TryAddShieldOnEnemyTurn(out var enemyShieldBlock))
        {
            var enemyShieldAction = new AddShieldOnEnemyTurnAction(this, enemyShieldBlock);
            actionsList.Insert(0, enemyShieldAction);
        }
        else if (enemy.actionHandler.TryAddSwordBlockOnEnemyTurn(out var enemySwordBlock))
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

        var mineStringBuilder = new StringBuilder();
        var enemyStringBuilder = new StringBuilder();

        var minePlayer = unit is Player ? unit as Player : null;
        var enemyPlayer = enemy is Player ? enemy as Player : null;

        if (minePlayer is not null)
        {
            mineStringBuilder.AppendLine(Localization.Get(minePlayer.session, "battle_mine_turn_maded"));
        }
        if (enemyPlayer is not null)
        {
            enemyStringBuilder.AppendLine(Localization.Get(enemyPlayer.session, "battle_enemy_turn_maded", unit.nickname));
        }

        foreach (var action in _battleActions)
        {
            action.ApplyActionWithMineStats(unit.unitStats);
            action.ApplyActionWithEnemyStats(enemy.unitStats);

            if (minePlayer is not null)
            {
                mineStringBuilder.AppendLine();
                mineStringBuilder.AppendLine(action.GetHeader(minePlayer.session));
                mineStringBuilder.AppendLine(action.GetDescription(minePlayer.session));
            }
            if (enemyPlayer is not null)
            {
                enemyStringBuilder.AppendLine();
                enemyStringBuilder.AppendLine(action.GetHeader(enemyPlayer.session));
                enemyStringBuilder.AppendLine(action.GetDescription(enemyPlayer.session));
            }            
        }

        if (minePlayer is not null)
        {
            mineStringBuilder.AppendLine();
            mineStringBuilder.AppendLine(battle.GetStatsView(minePlayer.session));
            var keyboard = BattleToolipHelper.GetStatsKeyboard(minePlayer.session);
            await messageSender.SendTextMessage(minePlayer.session.chatId, mineStringBuilder.ToString(), keyboard,
                cancellationToken: minePlayer.session.cancellationToken).FastAwait();
        }
        if (enemyPlayer is not null)
        {
            enemyStringBuilder.AppendLine();
            enemyStringBuilder.AppendLine(battle.GetStatsView(enemyPlayer.session));
            var keyboard = BattleToolipHelper.GetStatsKeyboard(enemyPlayer.session);
            await messageSender.SendTextMessage(enemyPlayer.session.chatId, enemyStringBuilder.ToString(), keyboard,
                cancellationToken: enemyPlayer.session.cancellationToken).FastAwait();
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
            _queryTooltipsToIgnoreByPlayers[player] = new();
        }
        var ingoreList = _queryTooltipsToIgnoreByPlayers[player];
        if (ingoreList.Contains(callback.tooltip))
        {
            await messageSender.AnswerQuery(queryId, cancellationToken: player.session.cancellationToken).FastAwait();
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
        var text = targetUnit.GetFullUnitInfoView(player.session);
        await messageSender.SendTextMessage(player.session.chatId, text, cancellationToken: player.session.cancellationToken).FastAwait();
        await messageSender.AnswerQuery(queryId, cancellationToken: player.session.cancellationToken).FastAwait();
    }



}
