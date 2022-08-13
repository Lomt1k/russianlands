using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Battle
{
    public enum BattleResult
    {
        Draw = 0,
        Win = 1,
        Lose = 2
    }

    public struct BattleResultData
    {
        public BattleResult resultType;
        public IEnumerable<RewardBase>? rewards;
        public Action<GameSession>? continueCallback;
        public bool isReturnToTownAvailable;
    }
}
