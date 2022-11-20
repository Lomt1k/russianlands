using System;
using System.Text;
using TextGameRPG.Scripts.GameCore.Localizations;
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
            resistance = new Items.DamageInfo(
                physicalDamage: (int)Math.Round(statsSettings.physicalResist * gradeMult),
                fireDamage: (int)Math.Round(statsSettings.fireResist * gradeMult),
                coldDamage: (int)Math.Round(statsSettings.coldResist * gradeMult),
                lightningDamage: (int)Math.Round(statsSettings.lightningResist * gradeMult));
        }

    }
}
