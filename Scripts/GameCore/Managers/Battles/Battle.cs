using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.Bot.CallbackData;
using TextGameRPG.Scripts.Bot.Dialogs.Battle;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Managers.Battles
{
    public enum BattleType { PVE, PVP }

    public class Battle
    {
        private List<RewardBase>? _rewards;
        private Func<Player, BattleResult, Task>? _onBattleEndFunc;
        private Func<Player, BattleResult, Task>? _onContinueButtonFunc;
        private Func<Player, BattleResult, bool>? _isAvailableReturnToTownFunc;

        private CancellationTokenSource _forceBattleCTS;

        public BattleType battleType { get; }
        public IBattleUnit firstUnit { get; private set; }
        public IBattleUnit secondUnit { get; private set; }
        public BattleTurn? currentTurn { get; private set; }
        public bool isPVE { get; private set; }
        public DateTime startTime { get; private set; }

        public Battle(Player opponentA, IBattleUnit opponentB,
            List<RewardBase>? rewards = null,
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
                if (firstPlayer.session.IsTasksCancelled())
                    return true;
            }
            if (secondUnit is Player secondPlayer)
            {
                if (secondPlayer.session.IsTasksCancelled())
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
            var battleTurnTimeInSeconds = isPVE ? 180 : 80;
            while (!HasDefeatedUnits())
            {
                currentTurn = new BattleTurn(this, firstUnit, battleTurnTimeInSeconds);
                await currentTurn.HandleTurn().FastAwait();
                currentTurn = new BattleTurn(this, secondUnit, battleTurnTimeInSeconds);
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
            Program.logger.Debug($"Message from {player.session.actualUser}: {callback.tooltip}");
            if (currentTurn == null)
            {
                await TelegramBot.instance.messageSender.AnswerQuery(player.session.chatId, queryId).FastAwait();
                return;
            }
            await currentTurn.HandleBattleTooltipCallback(player, queryId, callback).FastAwait();
        }

        private async Task BattleEnd()
        {
            if (IsCancellationRequested())
            {
                GlobalManagers.battleManager?.UnregisterBattle(this);
                return;
            }

            bool hasWinner = firstUnit.unitStats.currentHP > 0 || secondUnit.unitStats.currentHP > 0;
            if (firstUnit is Player firstPlayer)
            {
                await HandleBattleEndForPlayer(firstPlayer, hasWinner).FastAwait();
            }
            if (secondUnit is Player secondPlayer)
            {
                await HandleBattleEndForPlayer(secondPlayer, hasWinner).FastAwait();
            }
            GlobalManagers.battleManager?.UnregisterBattle(this);
        }

        private async Task HandleBattleEndForPlayer(Player player, bool hasWinner)
        {
            var enemy = GetEnemy(player);
            BattleResult battleResult = hasWinner
                ? (player.unitStats.currentHP > enemy.unitStats.currentHP ? BattleResult.Win : BattleResult.Lose)
                : BattleResult.Draw;

            await HandleBattleEndForPlayer(player, battleResult).FastAwait();
        }

        // Вызывается при ошибке (либо читом)
        public async Task ForceBattleEndWithResult(Player player, BattleResult battleResult)
        {
            GlobalManagers.battleManager?.UnregisterBattle(this);
            _forceBattleCTS.Cancel();

            if (!player.session.IsTasksCancelled())
            {
                await HandleBattleEndForPlayer(player, battleResult).FastAwait();
            }

            var enemy = GetEnemy(player);
            if (enemy is Player anotherPlayer)
            {
                if (!anotherPlayer.session.IsTasksCancelled())
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

            bool hasContinueButton = _onContinueButtonFunc != null;
            bool isReturnToTownAvailable = hasContinueButton
                ? _isAvailableReturnToTownFunc == null ? false : _isAvailableReturnToTownFunc(player, battleResult)
                : true;

            var data = new BattleResultData
            {
                battleResult = battleResult,
                rewards = _rewards,
                onContinueButtonFunc = _onContinueButtonFunc,
                isReturnToTownAvailable = isReturnToTownAvailable
            };
            await new BattleResultDialog(player.session, data).Start().FastAwait();
        }




    }
}
