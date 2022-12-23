﻿using Newtonsoft.Json;
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
        public int constructionTime;
        public bool isBoostAvailable;
        public int requiredGold;
        public int requiredHerbs;
        public int requiredWood;
    }

    [JsonObject]
    public class StorageLevelInfo : BuildingLevelInfo
    {
        public int resourceStorageLimit;
    }

    [JsonObject]
    public class ProductionLevelInfo : BuildingLevelInfo
    {
        public int productionPerHour;
        public int resourceStorageLimit;
        public float bonusPerWorkerLevel;
    }

    [JsonObject]
    public class TrainingLevelInfo : BuildingLevelInfo
    {
        public int maxUnitLevel;
    }

    [JsonObject]
    public class CraftLevelInfo : BuildingLevelInfo
    {
        public int rareCraft_HerbsCost;
        public int rareCraft_MaterialsCost;
        public int rareCraft_Time;
        public int rareCraft_MinLevel;
        public int rareCraft_MaxLevel;

        public int epicCraft_HerbsCost;
        public int epicCraft_MaterialsCost;
        public int epicCraft_Time;
        public int epicCraft_MinLevel;
        public int epicCraft_MaxLevel;

        public bool legendaryCraft_allowed = true;
        public int legendaryCraft_HerbsCost;
        public int legendaryCraft_MaterialsCost;
        public int legendaryCraft_Time;
        public int legendaryCraft_MinLevel;
        public int legendaryCraft_MaxLevel;
    }

    [JsonObject]
    public class HospitalLevelInfo : BuildingLevelInfo
    {
        public int restoreHealthPerMinute;
    }

    [JsonObject]
    public class TyrLevelInfo : BuildingLevelInfo
    {
        public int arrowsAmount;
    }


}
