﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public interface IBattleUnit
    {
        public string nickname { get; }
        public UnitStats unitStats { get; }
        public GameSession session { get; }

        public string GetStartTurnView(GameSession session);

        public void OnStartBattle(Battle battle);
        public Task<List<IBattleAction>> GetActionsForBattleTurn(BattleTurn battleTurn);
        public Task OnStartEnemyTurn(BattleTurn battleTurn);
        public void OnMineBattleTurnAlmostEnd();
        public Task OnMineBatteTurnTimeEnd();
        public Task OnEnemyBattleTurnTimeEnd();
    }
}
