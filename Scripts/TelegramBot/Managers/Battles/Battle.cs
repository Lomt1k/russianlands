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
        public IBattleUnit firstUnit { get; private set; }
        public IBattleUnit secondUnit { get; private set; }
        public BattleTurn? currentTurn { get; private set; }

        public Battle(Player opponentA, IBattleUnit opponentB)
        {
            firstUnit = SelectFirstUnit(opponentA, opponentB);
            secondUnit = firstUnit == opponentA ? opponentB : opponentA;
        }

        public abstract IBattleUnit SelectFirstUnit(Player opponentA, IBattleUnit opponentB);

        public void StartBattle()
        {
            firstUnit.OnStartBattle(this);
            secondUnit.OnStartBattle(this);

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
            return firstUnit.unitStats.currentHP < 1 || secondUnit.unitStats.currentHP < 1;
        }

    }
}
