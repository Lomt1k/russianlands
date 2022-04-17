using Newtonsoft.Json;
using JsonKnownTypes;

namespace TextGameRPG.Scripts.GameCore.Items.ItemGenerators.ItemPropertyGenerators
{
    using ItemProperties;

    [JsonConverter(typeof(JsonKnownTypesConverter<ItemPropertyGeneratorBase>))]
    public abstract class ItemPropertyGeneratorBase
    {
        public virtual string debugDescription { get; }
        public virtual ItemPropertyGeneratorType propertyType { get; }

        public abstract ItemPropertyBase Generate();
        public abstract ItemPropertyGeneratorBase Clone();
    }
}
