using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public class BattlePVP : Battle
    {
        public override BattleType battleType => BattleType.PVP;

        public BattlePVP(Player opponentA, Player opponentB) : base(opponentA, opponentB)
        {
        }

    }
}
