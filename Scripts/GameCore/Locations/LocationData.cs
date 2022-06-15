using System;
using System.Text.Json.Serialization;
using TextGameRPG.Scripts.GameCore.GameDataBase;

namespace TextGameRPG.Scripts.GameCore.Locations
{
    [Serializable]
    public class LocationData : IDataWithIntegerID
    {
        public int id { get; set; }
        [JsonIgnore] public string debugName => ((LocationType)id).ToString();

        //--- data:
        public int foodExplorePrice;

        public ItemGenerationSettings itemGenerationSettings = new ItemGenerationSettings();

        public LocationData Clone()
        {
            var clone = (LocationData)MemberwiseClone();
            clone.id = id;
            return clone;
        }
    }

    [Serializable]
    public class ItemGenerationSettings
    {
        public int basisPoints;
        public int levelForZeroGrade;
        public float increaseLevelByGrade;
        public float increaseLevelByRarity;
    }

}
