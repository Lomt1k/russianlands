using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public class BattleTurn
    {
        private Dictionary<IBattleUnit, List<IBattleAction>> _actionsByUnit;

        public Battle battle { get; }
        public int turnId { get; }
        public int maxSeconds { get; } = 60;

        public BattleTurn(Battle _battle, int _turnId)
        {
            battle = _battle;
            turnId = _turnId;
            _actionsByUnit = new Dictionary<IBattleUnit, List<IBattleAction>>();
        }

        public async Task HandleTurn()
        {
            AddManaPoint();
            //foreach (var unit in battle.units)
            //{
            //    AskUnitForBattleActions(unit);
            //}
            await WaitAnswersFromAllUnits();
            //TODO
        }

        private void AddManaPoint()
        {
            //foreach (var unit in battle.units)
            //{
            //    unit.unitStats.AddManaPoint();
            //}
        }

        private async void AskUnitForBattleActions(IBattleUnit unit)
        {
            _actionsByUnit[unit] = await unit.GetActionsForBattleTurn(this);
        }

        private async Task WaitAnswersFromAllUnits()
        {
            //while (_actionsByUnit.Count < battle.units.Length)
            while (true)
            {
                await Task.Delay(1000);
            }
        }



    }
}
