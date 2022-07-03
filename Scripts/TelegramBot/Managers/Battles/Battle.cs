using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public enum BattleType { PVP, PVE }

    public abstract class Battle
    {
        public abstract BattleType battleType { get; }
        public IBattleUnit[] units { get; protected set; }
        public BattleTurn? currentTurn { get; private set; }

        public Battle(Player opponentA, IBattleUnit opponentB)
        {
            units = new IBattleUnit[] { opponentA, opponentB };
            foreach (var unit in units)
            {
                unit.unitStats.OnBattleStart();
            }
        }

        public async Task StartBattleAsync()
        {
            await HandleBattle();
        }

        public async Task HandleBattle()
        {
            while (!HasDefeatedUnits())
            {
                currentTurn = new BattleTurn(this);
                await currentTurn.HandleTurn();
            }
            //TODO
        }

        private bool HasDefeatedUnits()
        {
            foreach (var unit in units)
            {
                if (unit.unitStats.currentHP < 1)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
