using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public class BattleTurn
    {
        private Battle _battle;

        public BattleTurn(Battle battle)
        {
            _battle = battle;
        }

        public async Task HandleTurn()
        {
            AddManaPoint();
            //TODO
        }

        private void AddManaPoint()
        {
            foreach (var unit in _battle.units)
            {
                unit.unitStats.AddManaPoint();
            }
        }



    }
}
