﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public class BattlePVE : Battle
    {
        public override BattleType battleType => BattleType.PVE;

        public BattlePVE(Player player, Mob mob) : base(player, mob)
        {
        }

        public override IBattleUnit SelectFirstUnit(Player opponentA, IBattleUnit opponentB)
        {
            return opponentA;
        }
    }
}
