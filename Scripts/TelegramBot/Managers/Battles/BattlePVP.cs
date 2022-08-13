using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Battle;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public class BattlePVP : Battle
    {
        public override BattleType battleType => BattleType.PVP;

        public BattlePVP(Player opponentA, Player opponentB,
            IEnumerable<RewardBase>? rewards = null,
            Func<Player, BattleResult, Task>? onBattleEndFunc = null,
            Func<Player, BattleResult, Task>? onContinueButtonFunc = null,
            Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc = null)
            : base(opponentA, opponentB, rewards, onBattleEndFunc, onContinueButtonFunc, isAvailableReturnToTownFunc)
        {
        }

        public override IBattleUnit SelectFirstUnit(Player opponentA, IBattleUnit opponentB)
        {
            return new Random().Next(2) == 0 ? opponentA : opponentB;
        }
    }
}
