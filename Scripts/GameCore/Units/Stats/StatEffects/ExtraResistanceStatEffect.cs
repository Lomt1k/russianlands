using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Units.Stats.StatEffects;

public class ExtraResistanceStatEffect : StatEffectBase
{
    public DamageInfo resistance { get; }
    public byte turnsCount;

    public ExtraResistanceStatEffect(DamageInfo _resistance, byte _turnsCount)
    {
        resistance = _resistance;
        turnsCount = _turnsCount;
    }
}
