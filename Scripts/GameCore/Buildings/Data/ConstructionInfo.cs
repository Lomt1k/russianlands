using Newtonsoft.Json;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.GameDataBase;

namespace TextGameRPG.Scripts.GameCore.Buildings.Data
{
    [JsonObject]
    public class ConstructionInfo : IDataWithIntegerID
    {
        public int id { get; set; }
        [JsonIgnore]
        public BuildingType buildingType => (BuildingType)id;
        [JsonIgnore]
        public string debugName => ((BuildingType)id).ToString();

        public List<ConstructionLevelInfo> levels { get; set; } = new List<ConstructionLevelInfo>();

        public void OnSetupAppMode(AppMode appMode)
        {
        }
    }

    [JsonObject]
    public class ConstructionLevelInfo
    {
        public int requiredTownHall;
        public int constructionTimeInSeconds;
        public bool isBoostAvailable;
        public int exp;
        public int requiredGold;
        public int requiredHerbs;
        public int requiredWood;
    }
}
