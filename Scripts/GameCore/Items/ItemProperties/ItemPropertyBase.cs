using Newtonsoft.Json;
using JsonKnownTypes;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    [JsonConverter(typeof(JsonKnownTypesConverter<ItemPropertyBase>))]
    public abstract class ItemPropertyBase
    {
        public virtual string debugDescription { get; }
        public virtual ItemPropertyType propertyType { get; }
    }

}
