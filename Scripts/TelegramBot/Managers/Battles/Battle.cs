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

    public abstract class Battle
    {
        private IEnumerable<RewardBase>? _rewards;
        private Func<Player, BattleResult, Task>? _onBattleEndFunc;
        private Func<Player, BattleResult, Task>? _onContinueButtonFunc;
        private Func<Player, BattleResult, bool>? _isAvailableReturnToTownFunc;

        public abstract BattleType battleType { get; }
        public IBattleUnit firstUnit { get; private set; }
        public IBattleUnit secondUnit { get; private set; }
        public BattleTurn? currentTurn { get; private set; }

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

        public abstract IBattleUnit SelectFirstUnit(Player opponentA, IBattleUnit opponentB);

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
            //TODO: Battle end logic
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

    }
}
