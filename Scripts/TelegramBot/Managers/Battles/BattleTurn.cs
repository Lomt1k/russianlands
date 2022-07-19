using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public class BattleTurn
    {
        private Battle _battle;
        private Dictionary<IBattleUnit, List<IBattleAction>> _actionsByUnit;

        public BattleTurn(Battle battle)
        {
            _battle = battle;
            _actionsByUnit = new Dictionary<IBattleUnit, List<IBattleAction>>();
        }

        public async Task HandleTurn()
        {
            AddManaPoint();
            foreach (var unit in _battle.units)
            {
                AskUnitForBattleActions(unit);
            }
            await WaitAnswersFromAllUnits();
            //TODO
        }

        private void AddManaPoint()
        {
            foreach (var unit in _battle.units)
            {
                unit.unitStats.AddManaPoint();
            }
        }

        private async void AskUnitForBattleActions(IBattleUnit unit)
        {
            _actionsByUnit[unit] = await unit.GetActionsForBattleTurn(maxSeconds: 60);
        }

        private async Task WaitAnswersFromAllUnits()
        {
            while (_actionsByUnit.Count < _battle.units.Length)
            {
                await Task.Delay(1000);
            }
        }



    }
}
