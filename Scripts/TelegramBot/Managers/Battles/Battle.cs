using System;
using System.Collections.Generic;
using System.Text;
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
        private IEnumerable<RewardBase>? _rewards;
        private Func<Player, BattleResult, Task>? _onBattleEndFunc;
        private Func<Player, BattleResult, Task>? _onContinueButtonFunc;
        private Func<Player, BattleResult, bool>? _isAvailableReturnToTownFunc;

        public BattleType battleType { get; }
        public IBattleUnit firstUnit { get; private set; }
        public IBattleUnit secondUnit { get; private set; }
        public BattleTurn? currentTurn { get; private set; }
        public bool isPVE => secondUnit is Mob;

        public Battle(Player opponentA, IBattleUnit opponentB,
            IEnumerable<RewardBase>? rewards = null,
            Func<Player, BattleResult, Task>? onBattleEndFunc = null,
            Func<Player, BattleResult, Task>? onContinueButtonFunc = null,
            Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc = null)
        {
            firstUnit = SelectFirstUnit(opponentA, opponentB);
            secondUnit = firstUnit == opponentA ? opponentB : opponentA;

            _rewards = rewards;
            _onBattleEndFunc = onBattleEndFunc;
            _onContinueButtonFunc = onContinueButtonFunc;
            _isAvailableReturnToTownFunc = isAvailableReturnToTownFunc;
        }

        public IBattleUnit SelectFirstUnit(Player opponentA, IBattleUnit opponentB)
        {
            return isPVE ? opponentA : new Random().Next(2) == 0 ? opponentA : opponentB;
        }

        public void StartBattle()
        {
            firstUnit.OnStartBattle(this);
            secondUnit.OnStartBattle(this);

            HandleBattleAsync();
        }

        private async void HandleBattleAsync()
        {
            while (!HasDefeatedUnits())
            {
                currentTurn = new BattleTurn(this, firstUnit);
                await currentTurn.HandleTurn();
                currentTurn = new BattleTurn(this, secondUnit);
                await currentTurn.HandleTurn();
            }
            currentTurn = null;
            await BattleEnd();
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
            if (currentTurn == null)
            {
                await TelegramBot.instance.messageSender.AnswerQuery(queryId);
                return;
            }
            await currentTurn.HandleBattleTooltipCallback(player, queryId, callback);
        }

        private async Task BattleEnd()
        {
            bool hasWinner = firstUnit.unitStats.currentHP > 0 || secondUnit.unitStats.currentHP > 0;
            if (firstUnit is Player firstPlayer)
            {
                await HandleBattleEndForPlayer(firstPlayer, hasWinner);
            }
            if (secondUnit is Player secondPlayer)
            {
                await HandleBattleEndForPlayer(secondPlayer, hasWinner);
            }
            GlobalManagers.battleManager?.OnBattleEnd(this);
        }

        private async Task HandleBattleEndForPlayer(Player player, bool hasWinner)
        {
            var enemy = GetEnemy(player);
            BattleResult battleResult = hasWinner
                ? (player.unitStats.currentHP > enemy.unitStats.currentHP ? BattleResult.Win : BattleResult.Lose)
                : BattleResult.Draw;

            if (battleResult == BattleResult.Win)
            {
                GiveRewards(player);
            }

            if (_onBattleEndFunc != null)
            {
                await _onBattleEndFunc(player, battleResult);
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
            await new BattleResultDialog(player.session, data).Start();
        }

        private void GiveRewards(Player player)
        {
            if (_rewards == null)
                return;

            foreach (var reward in _rewards)
            {
                reward.AddReward(player);
            }
        }


    }
}
