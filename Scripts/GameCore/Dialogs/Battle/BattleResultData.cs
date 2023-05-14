using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Dialogs.Battle;

public enum BattleResult : byte
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
