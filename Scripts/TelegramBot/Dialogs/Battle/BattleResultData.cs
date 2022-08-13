using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Units;

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
        public BattleResult battleResult;
        public IEnumerable<RewardBase>? rewards;
        public Func<Player, BattleResult, Task>? onContinueButtonFunc;
        public bool isReturnToTownAvailable;
    }
}
