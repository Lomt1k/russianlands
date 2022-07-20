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

        public void StartBattle()
        {
            HandleBattleAsync();
        }

        public async void HandleBattleAsync()
        {
            int turnId = 0;
            while (!HasDefeatedUnits())
            {
                turnId++;
                currentTurn = new BattleTurn(this, turnId);
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
