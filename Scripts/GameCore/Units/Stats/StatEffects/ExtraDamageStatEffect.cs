using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Units.Stats.StatEffects
{
    public class ExtraDamageStatEffect : StatEffectBase
    {
        public DamageInfo damageInfo { get; }

        public ExtraDamageStatEffect(DamageInfo _damageInfo)
        {
            damageInfo = _damageInfo;
        }
    }
}
