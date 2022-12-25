using JsonKnownTypes;
using Newtonsoft.Json;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.GameDataBase;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    [JsonConverter(typeof(JsonKnownTypesConverter<PotionData>))]
    public abstract class PotionData : IDataWithIntegerID
    {
        public string debugName { get; set; } = "New Potion";
        public int id { get; set; }
        public byte workshopLevel;
        public string localizationKey = string.Empty;
        public byte potionLevel;

        // костыль для отображения в редакторе
        public byte workshopLevelProperty => workshopLevel;

        public PotionData(int _id)
        {
            id = _id;
        }

        public PotionData Clone()
        {
            return (PotionData)MemberwiseClone();
        }

        public void OnSetupAppMode(AppMode appMode)
        {
            //ignored
        }

        public abstract string GetView(GameSession session);

    }
}
