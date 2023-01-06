using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Units.Stats.StatEffects;

public static class StatEffectsListExtensions
{
    public static void OnResistanceStatEffectsUsed(this List<StatEffectBase> statEffects)
    {
        var listToRemove = new List<ExtraResistanceStatEffect>();
        foreach (var effect in statEffects)
        {
            if (effect is ExtraResistanceStatEffect resistanceEffect)
            {
                resistanceEffect.turnsCount--;
                if (resistanceEffect.turnsCount == 0)
                {
                    listToRemove.Add(resistanceEffect);
                }
            }
        }
        foreach (var effect in listToRemove)
        {
            statEffects.Remove(effect);
        }
    }

    public static DamageInfo GetExtraDamageAndRemoveEffects(this List<StatEffectBase> statEffects)
    {
        var listToRemove = new List<ExtraDamageStatEffect>();
        var result = new DamageInfo(0);
        foreach (var effect in statEffects)
        {
            if (effect is ExtraDamageStatEffect damageEffect)
            {
                result += damageEffect.damageInfo;
                listToRemove.Add(damageEffect);
            }
        }
        foreach (var effect in listToRemove)
        {
            statEffects.Remove(effect);
        }
        return result;
    }


}
