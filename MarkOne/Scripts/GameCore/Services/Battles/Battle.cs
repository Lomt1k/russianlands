using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.CallbackData;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Services.Battles;

public enum BattleType { PVE, PVP }

public class Battle
{
    private static readonly BattleManager battleManager = ServiceLocator.Get<BattleManager>();
    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    private readonly IEnumerable<RewardBase>? _rewards;
    private readonly Func<Player, BattleResult, Task>? _onBattleEndFunc;
    private readonly Func<Player, BattleResult, Task>? _onContinueButtonFunc;
    private readonly Func<Player, BattleResult, bool>? _isAvailableReturnToTownFunc;

    private readonly CancellationTokenSource _forceBattleCTS;

    public BattleType battleType { get; }
    public IBattleUnit firstUnit { get; private set; }
    public IBattleUnit secondUnit { get; private set; }
    public BattleTurn? currentTurn { get; private set; }
    public bool isPVE { get; private set; }
    public DateTime startTime { get; private set; }

    public Battle(Player opponentA, IBattleUnit opponentB, IEnumerable<RewardBase>? rewards = null,
        Func<Player, BattleResult, Task>? onBattleEndFunc = null,
        Func<Player, BattleResult, Task>? onContinueButtonFunc = null,
        Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc = null)
    {
        isPVE = opponentB is Mob;
        startTime = DateTime.UtcNow;
        firstUnit = SelectFirstUnit(opponentA, opponentB);
        secondUnit = firstUnit == opponentA ? opponentB : opponentA;

        _forceBattleCTS = new CancellationTokenSource();

        _rewards = rewards;
        _onBattleEndFunc = onBattleEndFunc;
        _onContinueButtonFunc = onContinueButtonFunc;
        _isAvailableReturnToTownFunc = isAvailableReturnToTownFunc;
    }

    public bool IsCancellationRequested()
    {
        if (_forceBattleCTS.IsCancellationRequested)
            return true;
        if (firstUnit is Player firstPlayer)
        {
            if (firstPlayer.session.cancellationToken.IsCancellationRequested)
                return true;
        }
        if (secondUnit is Player secondPlayer)
        {
            if (secondPlayer.session.cancellationToken.IsCancellationRequested)
                return true;
        }
        return false;
    }

    public IBattleUnit SelectFirstUnit(Player opponentA, IBattleUnit opponentB)
    {
        return isPVE ? opponentA : new Random().Next(2) == 0 ? opponentA : opponentB;
    }

    public async Task StartBattle()
    {
        if (IsCancellationRequested())
            return;

        InvokeHealthRegen();

        var firstUnitForLog = firstUnit is Player firtPlayer ? firtPlayer.session.actualUser.ToString() : firstUnit.nickname;
        var secondUnitForLog = secondUnit is Player secondPlayer ? secondPlayer.session.actualUser.ToString() : secondUnit.nickname;
        Program.logger.Info($"BATTLE | New {(isPVE ? "PVE" : "PVP")} battle started: {firstUnitForLog} vs {secondUnitForLog}");

        //Сначала второму юниту, так как первый уже сразу сможет ходить
        await secondUnit.OnStartBattle(this).FastAwait();
        await firstUnit.OnStartBattle(this).FastAwait();

        HandleBattleAsync();
    }

    private void InvokeHealthRegen()
    {
        if (firstUnit is Player firstPlayer)
        {
            firstPlayer.healhRegenerationController.InvokeRegen();
        }
        if (secondUnit is Player secondPlayer)
        {
            secondPlayer.healhRegenerationController.InvokeRegen();
        }
    }

    private async void HandleBattleAsync()
    {
        var battleTurnTimeInSeconds = isPVE ? 120 : 80;
        int turnNumber = 0;
        while (!HasDefeatedUnits())
        {
            turnNumber++;
            currentTurn = new BattleTurn(this, firstUnit, battleTurnTimeInSeconds, _isFirstUnit: true, turnNumber);
            await currentTurn.HandleTurn().FastAwait();
            currentTurn = new BattleTurn(this, secondUnit, battleTurnTimeInSeconds, _isFirstUnit: false, turnNumber);
            await currentTurn.HandleTurn().FastAwait();
        }
        currentTurn = null;
        await BattleEnd().FastAwait();
    }

    public string GetStatsView(GameSession session)
    {
        var mineUnit = session.player;
        var enemyUnit = GetEnemy(mineUnit);
        var sb = new StringBuilder();

        sb.AppendLine(Localization.Get(session, "battle_header_your_health"));
        sb.AppendLine(Emojis.StatHealth + $"{mineUnit.unitStats.currentHP} / {mineUnit.unitStats.maxHP}");

        sb.AppendLine(Localization.Get(session, "battle_header_enemy_health"));
        sb.AppendLine(Emojis.StatHealth + $"{enemyUnit.unitStats.currentHP} / {enemyUnit.unitStats.maxHP}");

        return sb.ToString();
    }

    public IBattleUnit GetEnemy(IBattleUnit unit)
    {
        return unit == firstUnit ? secondUnit : firstUnit;
    }

    private bool HasDefeatedUnits()
    {
        return firstUnit.unitStats.currentHP < 1 || secondUnit.unitStats.currentHP < 1;
    }

    public async Task HandleBattleTooltipCallback(Player player, string queryId, BattleTooltipCallbackData callback)
    {
        if (BotController.config.logSettings.logUpdates)
        {
            Program.logger.Info($"UPDATE :: {player.session.actualUser}: {callback.tooltip}");
        }

        if (currentTurn == null)
        {
            await messageSender.AnswerQuery(queryId, cancellationToken: player.session.cancellationToken).FastAwait();
            return;
        }
        await currentTurn.HandleBattleTooltipCallback(player, queryId, callback).FastAwait();
    }

    private async Task BattleEnd()
    {
        if (IsCancellationRequested())
        {
            battleManager.UnregisterBattle(this);
            return;
        }

        // ВАЖНО: Результаты должны быть посчитаны до вызова HandleBattleEndForPlayer
        var hasWinner = firstUnit.unitStats.currentHP > 0 || secondUnit.unitStats.currentHP > 0;
        var firstUnitResult = hasWinner
            ? (firstUnit.unitStats.currentHP > secondUnit.unitStats.currentHP ? BattleResult.Win : BattleResult.Lose)
            : BattleResult.Draw;
        var secondUnitResult = hasWinner
            ? (secondUnit.unitStats.currentHP > firstUnit.unitStats.currentHP ? BattleResult.Win : BattleResult.Lose)
            : BattleResult.Draw;

        if (firstUnit is Player firstPlayer)
        {
            await HandleBattleEndForPlayer(firstPlayer, firstUnitResult).FastAwait();
        }
        if (secondUnit is Player secondPlayer)
        {
            await HandleBattleEndForPlayer(secondPlayer, secondUnitResult).FastAwait();
        }
        battleManager.UnregisterBattle(this);
    }

    // Вызывается при ошибке (либо читом)
    public async Task ForceBattleEndWithResult(Player player, BattleResult battleResult)
    {
        battleManager.UnregisterBattle(this);
        _forceBattleCTS.Cancel();

        if (!player.session.cancellationToken.IsCancellationRequested)
        {
            await HandleBattleEndForPlayer(player, battleResult).FastAwait();
        }

        var enemy = GetEnemy(player);
        if (enemy is Player anotherPlayer)
        {
            if (!anotherPlayer.session.cancellationToken.IsCancellationRequested)
            {
                if (battleResult == BattleResult.Win) battleResult = BattleResult.Lose;
                else if (battleResult == BattleResult.Lose) battleResult = BattleResult.Win;
                await HandleBattleEndForPlayer(anotherPlayer, battleResult).FastAwait();
            }
        }
    }

    private async Task HandleBattleEndForPlayer(Player player, BattleResult battleResult)
    {
        player.OnBattleEnd(this, battleResult);
        if (_onBattleEndFunc != null)
        {
            await _onBattleEndFunc(player, battleResult).FastAwait();
        }

        var hasContinueButton = _onContinueButtonFunc != null;
        var isReturnToTownAvailable = !hasContinueButton || (_isAvailableReturnToTownFunc != null && _isAvailableReturnToTownFunc(player, battleResult));

        var data = new BattleResultData
        {
            battleResult = battleResult,
            rewards = _rewards,
            onContinueButtonFunc = _onContinueButtonFunc,
            isReturnToTownAvailable = isReturnToTownAvailable
        };

        Program.logger.Info($"BATTLE | User {player.session.actualUser} end {(isPVE ? "PVE" : "PVP")} battle with result: {battleResult}");
        await new BattleResultDialog(player.session, data).Start().FastAwait();
    }




}
