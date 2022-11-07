using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.TelegramBot.CallbackData;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Battle;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public enum BattleType { PVP, PVE }

    public class Battle
    {
        private List<RewardBase>? _rewards;
        private Func<Player, BattleResult, Task>? _onBattleEndFunc;
        private Func<Player, BattleResult, Task>? _onContinueButtonFunc;
        private Func<Player, BattleResult, bool>? _isAvailableReturnToTownFunc;

        private CancellationTokenSource _allSessionsCTS;
        private CancellationTokenSource _playerOneCTS;
        private CancellationTokenSource? _playerTwoCTS;
        private CancellationTokenSource _forceBattleCTS;

        public BattleType battleType { get; }
        public IBattleUnit firstUnit { get; private set; }
        public IBattleUnit secondUnit { get; private set; }
        public BattleTurn? currentTurn { get; private set; }
        public bool isPVE { get; private set; }

        public Battle(Player opponentA, IBattleUnit opponentB,
            List<RewardBase>? rewards = null,
            Func<Player, BattleResult, Task>? onBattleEndFunc = null,
            Func<Player, BattleResult, Task>? onContinueButtonFunc = null,
            Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc = null)
        {
            isPVE = opponentB is Mob;
            firstUnit = SelectFirstUnit(opponentA, opponentB);
            secondUnit = firstUnit == opponentA ? opponentB : opponentA;

            _allSessionsCTS = TelegramBot.instance.sessionManager.allSessionsTasksCTS;
            _playerOneCTS = opponentA.session.sessionTasksCTS;
            if (opponentB is Player secondPlayer)
            {
                _playerTwoCTS = secondPlayer.session.sessionTasksCTS;
            }
            _forceBattleCTS = new CancellationTokenSource();

            _rewards = rewards;
            _onBattleEndFunc = onBattleEndFunc;
            _onContinueButtonFunc = onContinueButtonFunc;
            _isAvailableReturnToTownFunc = isAvailableReturnToTownFunc;
        }

        public bool IsCancellationRequested()
        {
            return _allSessionsCTS.IsCancellationRequested || _playerOneCTS.IsCancellationRequested
            || (_playerTwoCTS != null && _playerTwoCTS.IsCancellationRequested)
            || _forceBattleCTS.IsCancellationRequested;
        }

        public IBattleUnit SelectFirstUnit(Player opponentA, IBattleUnit opponentB)
        {
            return isPVE ? opponentA : new Random().Next(2) == 0 ? opponentA : opponentB;
        }

        public async Task StartBattle()
        {
            if (IsCancellationRequested())
                return;

            //Сначала второму юниту, так как первый уже сразу сможет ходить
            await secondUnit.OnStartBattle(this).ConfigureAwait(false);
            await firstUnit.OnStartBattle(this).ConfigureAwait(false);

            HandleBattleAsync();
        }

        private async void HandleBattleAsync()
        {
            var battleTurnTimeInSeconds = isPVE ? 180 : 80;
            while (!HasDefeatedUnits())
            {
                currentTurn = new BattleTurn(this, firstUnit, battleTurnTimeInSeconds);
                await currentTurn.HandleTurn().ConfigureAwait(false);
                currentTurn = new BattleTurn(this, secondUnit, battleTurnTimeInSeconds);
                await currentTurn.HandleTurn().ConfigureAwait(false);
            }
            currentTurn = null;
            await BattleEnd().ConfigureAwait(false);
        }

        public string GetStatsView(GameSession session)
        {
            var mineUnit = session.player;
            var enemyUnit = GetEnemy(mineUnit);
            var sb = new StringBuilder();

            sb.AppendLine(Localization.Get(session, "battle_header_your_health"));
            sb.AppendLine($"{Emojis.stats[Stat.Health]} {mineUnit.unitStats.currentHP} / {mineUnit.unitStats.maxHP}");

            sb.AppendLine(Localization.Get(session, "battle_header_enemy_health"));
            sb.AppendLine($"{Emojis.stats[Stat.Health]} {enemyUnit.unitStats.currentHP} / {enemyUnit.unitStats.maxHP}");

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
                await TelegramBot.instance.messageSender.AnswerQuery(player.session.chatId, queryId)
                    .ConfigureAwait(false);
                return;
            }
            await currentTurn.HandleBattleTooltipCallback(player, queryId, callback)
                .ConfigureAwait(false);
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
                await HandleBattleEndForPlayer(firstPlayer, hasWinner).ConfigureAwait(false);
            }
            if (secondUnit is Player secondPlayer)
            {
                await HandleBattleEndForPlayer(secondPlayer, hasWinner).ConfigureAwait(false);
            }
            GlobalManagers.battleManager?.UnregisterBattle(this);
        }

        private async Task HandleBattleEndForPlayer(Player player, bool hasWinner)
        {
            var enemy = GetEnemy(player);
            BattleResult battleResult = hasWinner
                ? (player.unitStats.currentHP > enemy.unitStats.currentHP ? BattleResult.Win : BattleResult.Lose)
                : BattleResult.Draw;

            await HandleBattleEndForPlayer(player, battleResult).ConfigureAwait(false);
        }

        // Вызывается при ошибке (либо читом)
        public async Task ForceBattleEndWithResult(Player player, BattleResult battleResult)
        {
            GlobalManagers.battleManager?.UnregisterBattle(this);
            _forceBattleCTS.Cancel();

            var allSessionsCTS = TelegramBot.instance.sessionManager.allSessionsTasksCTS;
            if (allSessionsCTS.IsCancellationRequested)
                return;

            if (!player.session.sessionTasksCTS.IsCancellationRequested)
            {
                await HandleBattleEndForPlayer(player, battleResult).ConfigureAwait(false);
            }

            var enemy = GetEnemy(player);
            if (enemy is Player anotherPlayer)
            {
                if (!anotherPlayer.session.sessionTasksCTS.IsCancellationRequested)
                {
                    if (battleResult == BattleResult.Win) battleResult = BattleResult.Lose;
                    else if (battleResult == BattleResult.Lose) battleResult = BattleResult.Win;
                    await HandleBattleEndForPlayer(anotherPlayer, battleResult).ConfigureAwait(false);
                }                
            }
        }

        private async Task HandleBattleEndForPlayer(Player player, BattleResult battleResult)
        {
            if (_onBattleEndFunc != null)
            {
                await _onBattleEndFunc(player, battleResult).ConfigureAwait(false);
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
            await new BattleResultDialog(player.session, data).Start().ConfigureAwait(false);
        }




    }
}
