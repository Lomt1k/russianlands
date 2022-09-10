using Newtonsoft.Json;
using JsonKnownTypes;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.GameDataBase;

namespace TextGameRPG.Scripts.GameCore.Buildings.Data
{
    [JsonObject]
    public class BuildingData : IDataWithIntegerID
    {
        public int id { get; set; }
        [JsonIgnore]
        public BuildingType buildingType => (BuildingType)id;
        [JsonIgnore]
        public string debugName => ((BuildingType)id).ToString();

        public List<BuildingLevelInfo> levels { get; set; } = new List<BuildingLevelInfo>();

        public void OnSetupAppMode(AppMode appMode)
        {
        }

        public BuildingData Clone()
        {
            var clone = (BuildingData)MemberwiseClone();
            clone.id = id;
            return clone;
        }
    }

    [JsonObject]
    [JsonConverter(typeof(JsonKnownTypesConverter<BuildingLevelInfo>))]
    public class BuildingLevelInfo
    {
        public int requiredTownHall;
        public int constructionTimeInSeconds;
        public bool isBoostAvailable;
        public int exp;
        public int requiredGold;
        public int requiredHerbs;
        public int requiredWood;
    }

    [JsonObject]
    public class StorageLevelInfo : BuildingLevelInfo
    {
        public int maxResourceValue;
    }


}
