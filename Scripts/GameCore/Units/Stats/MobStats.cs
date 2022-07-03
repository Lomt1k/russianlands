
using System;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public class MobStats : UnitStats
    {
        public MobStats(MobData mobData, float gradeMult)
        {
            var statsSettings = mobData.statsSettings;
            maxHP = (int)Math.Round(statsSettings.health * gradeMult);
            currentHP = maxHP;
            physicalResist = (int)Math.Round(statsSettings.physicalResist * gradeMult);
            fireResist = (int)Math.Round(statsSettings.fireResist * gradeMult);
            coldResist = (int)Math.Round(statsSettings.coldResist * gradeMult);
            lightningResist = (int)Math.Round(statsSettings.lightningResist * gradeMult);            
        }

        public override string GetView()
        {
            return "MOB_STATS_VIEW [В РАЗРАБОТКЕ]";
        }
    }
}
