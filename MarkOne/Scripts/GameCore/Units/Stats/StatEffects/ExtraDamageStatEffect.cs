using MarkOne.Scripts.GameCore.Items;

namespace MarkOne.Scripts.GameCore.Units.Stats.StatEffects;

public class ExtraDamageStatEffect : StatEffectBase
{
    public DamageInfo damageInfo { get; }

    public ExtraDamageStatEffect(DamageInfo _damageInfo)
    {
        damageInfo = _damageInfo;
    }
}
