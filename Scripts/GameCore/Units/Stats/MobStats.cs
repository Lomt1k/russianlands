
namespace TextGameRPG.Scripts.GameCore.Units.Stats;

public class MobStats : UnitStats
{
    private readonly Mob _mob;

    public MobStats(Mob mob)
    {
        _mob = mob;
        var statsSettings = mob.mobData.statsSettings;

        maxHP = statsSettings.health;
        resistance = new Items.DamageInfo(
            physicalDamage: statsSettings.physicalResist,
            fireDamage: statsSettings.fireResist,
            coldDamage: statsSettings.coldResist,
            lightningDamage: statsSettings.lightningResist);
    }

}
