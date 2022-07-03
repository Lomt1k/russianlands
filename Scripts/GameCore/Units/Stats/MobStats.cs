
using System;
using System.Text;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public class MobStats : UnitStats
    {
        private Mob _mob;

        public MobStats(Mob mob)
        {
            _mob = mob;
            var statsSettings = mob.mobData.statsSettings;
            var gradeMult = mob.gradeMult;

            maxHP = (int)Math.Round(statsSettings.health * gradeMult);
            currentHP = maxHP;
            physicalResist = (int)Math.Round(statsSettings.physicalResist * gradeMult);
            fireResist = (int)Math.Round(statsSettings.fireResist * gradeMult);
            coldResist = (int)Math.Round(statsSettings.coldResist * gradeMult);
            lightningResist = (int)Math.Round(statsSettings.lightningResist * gradeMult);            
        }

        public override string GetView(GameSession session)
        {
            return "MOB_STATS_VIEW [В РАЗРАБОТКЕ]";
        }

    }
}
