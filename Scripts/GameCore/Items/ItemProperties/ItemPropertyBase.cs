using Newtonsoft.Json;
using JsonKnownTypes;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    [JsonConverter(typeof(JsonKnownTypesConverter<ItemPropertyBase>))]
    public abstract class ItemPropertyBase
    {
        [JsonIgnore] public abstract string debugDescription { get; }
        [JsonIgnore] public abstract ItemPropertyType propertyType { get; }

        public ItemPropertyBase Clone()
        {
            return (ItemPropertyBase)MemberwiseClone();
        }

    }

}
