using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public class BattleTurn
    {
        private List<IBattleAction> _battleActions = new List<IBattleAction>();

        public Battle battle { get; }
        public IBattleUnit unit { get; }
        public int secondsLimit { get; }
        public int secondsLeft { get; private set; }
        public bool isLastChance { get; }

        public bool isWaitingForActions => _battleActions.Count == 0 && secondsLeft > 0;

        public BattleTurn(Battle _battle, IBattleUnit _unit, int _secondsLimit = 60)
        {
            battle = _battle;
            unit = _unit;
            secondsLimit = _secondsLimit;
            secondsLeft = _secondsLimit;
            isLastChance = unit.unitStats.currentHP <= 0;
        }

        public async Task HandleTurn()
        {
            var enemy = battle.GetEnemy(unit);
            await enemy.OnStartEnemyTurn(this);

            AddManaPoint();
            AskUnitForBattleActions();
            await WaitAnswersFromAllUnits();
            //TODO: Apply actions
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
                _battleActions = selectedActions;
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
                if (secondsLeft == 0 && _battleActions.Count == 0)
                {
                    await unit.OnMineBatteTurnTimeEnd();
                    var enemy = battle.GetEnemy(unit);
                    await enemy.OnEnemyBattleTurnTimeEnd();
                }
            }
        }



    }
}
