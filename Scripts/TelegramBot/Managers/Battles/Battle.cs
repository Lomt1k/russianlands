using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public enum BattleType { PVP, PVE }

    public abstract class Battle
    {
        public abstract BattleType battleType { get; }

        public abstract Task StartBattleAsync();
    }
}
