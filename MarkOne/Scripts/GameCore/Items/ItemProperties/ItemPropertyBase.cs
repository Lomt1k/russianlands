using JsonKnownTypes;
using Newtonsoft.Json;
using System;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.ItemProperties;

[JsonConverter(typeof(JsonKnownTypesConverter<ItemPropertyBase>))]
public abstract class ItemPropertyBase
{
    [JsonIgnore]
    public abstract string debugDescription { get; }
    [JsonIgnore]
    public abstract PropertyType propertyType { get; }

    public ItemPropertyBase Clone()
    {
        return (ItemPropertyBase)MemberwiseClone();
    }

    public abstract string GetView(GameSession session);
    public abstract IEnumerable<ItemStatIcon> GetIcons(ItemType itemType);
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
