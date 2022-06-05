using Newtonsoft.Json;
using JsonKnownTypes;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    [JsonConverter(typeof(JsonKnownTypesConverter<ItemPropertyBase>))]
    public abstract class ItemPropertyBase
    {
        [JsonIgnore] public abstract string debugDescription { get; }
        [JsonIgnore] public abstract PropertyType propertyType { get; }
        [JsonIgnore] public abstract bool isSupportLevelUp { get; }

        public ItemPropertyBase Clone()
        {
            return (ItemPropertyBase)MemberwiseClone();
        }

        public abstract void ApplyItemLevel(byte level);
        public abstract string GetView(GameSession session);

    }

}
