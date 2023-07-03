using JsonKnownTypes;
using Newtonsoft.Json;
using System;
using MarkOne.Scripts.Utils;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities;

[JsonConverter(typeof(JsonKnownTypesConverter<ItemAbilityBase>))]
public abstract class ItemAbilityBase
{
    [JsonIgnore] public abstract string debugDescription { get; }
    [JsonIgnore] public abstract AbilityType abilityType { get; }

    public byte chanceToSuccessPercentage = 100;
    public sbyte manaCost;

    public ItemAbilityBase Clone()
    {
        return (ItemAbilityBase)MemberwiseClone();
    }

    public bool TryChance()
    {
        return Randomizer.TryPercentage(chanceToSuccessPercentage);
    }

    public override string ToString()
    {
        return debugDescription + (chanceToSuccessPercentage < 100 ? $" (Вероятность {chanceToSuccessPercentage}%)" : string.Empty);
    }

    public abstract string GetView(GameSession session);
    public virtual void ApplySkillLevel(byte level)
    {
        // ignored by default
    }

    protected void IncreaseByPercents(ref int value, byte percents)
    {
        value = value * (100 + percents);
        value = (int)Math.Round(value / 100f);
    }

}
