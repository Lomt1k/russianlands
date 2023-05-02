using JsonKnownTypes;
using Newtonsoft.Json;
using System;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties;

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
