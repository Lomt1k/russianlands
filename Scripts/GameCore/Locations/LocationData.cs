using System;
using TextGameRPG.Scripts.GameCore.GameDataBase;

namespace TextGameRPG.Scripts.GameCore.Locations
{
    [Serializable]
    public class LocationData : IDataWithIntegerID
    {
        public int id { get; }
        public string debugName => ((LocationType)id).ToString();
        
        public int foodExplorePrice;
        public ItemGenerationSettings itemGenerationSettings;
    }

    [Serializable]
    public struct ItemGenerationSettings
    {
        public int basisPoints;
        public int levelForZeroGrade;
        public float increaseLevelByGrade;
        public float increaseLevelByRarity;
    }

}
