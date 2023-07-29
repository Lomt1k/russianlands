using JsonKnownTypes;
using Newtonsoft.Json;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Quests;

namespace MarkOne.Scripts.GameCore.Buildings.Data;

[JsonObject]
public class BuildingData : IGameDataWithId<BuildingId>
{
    public BuildingId id { get; set; }
    [JsonIgnore]
    public string debugName => id.ToString();

    public List<BuildingLevelInfo> levels { get; set; } = new List<BuildingLevelInfo>();

    public void OnBotAppStarted()
    {
        // ignored
    }
}

[JsonObject]
[JsonConverter(typeof(JsonKnownTypesConverter<BuildingLevelInfo>))]
public class BuildingLevelInfo
{
    public int requiredTownHall { get; set; }
    public int constructionTime { get; set; }
    public bool isBoostAvailable { get; set; }
    public int requiredGold { get; set; }
    public int requiredHerbs { get; set; }
    public int requiredWood { get; set; }
}

[JsonObject]
public class TownhallLevelInfo : BuildingLevelInfo
{
    public QuestId requireCompletedQuest { get; set; }
}

[JsonObject]
public class StorageLevelInfo : BuildingLevelInfo
{
    public int resourceStorageLimit { get; set; }
}

[JsonObject]
public class ProductionLevelInfo : BuildingLevelInfo
{
    public int productionPerHour { get; set; }
    public int resourceStorageLimit { get; set; }
    public float bonusPerWorkerLevel { get; set; }
}

[JsonObject]
public class TrainingLevelInfo : BuildingLevelInfo
{
    public int maxUnitLevel { get; set; }
}

[JsonObject]
public class CraftLevelInfo : BuildingLevelInfo
{
    public int rareCraft_WoodCost { get; set; }
    public int rareCraft_MaterialsCost { get; set; }
    public int rareCraft_Time { get; set; }

    public int epicCraft_WoodCost { get; set; }
    public int epicCraft_MaterialsCost { get; set; }
    public int epicCraft_Time { get; set; }

    public int legendaryCraft_WoodCost { get; set; }
    public int legendaryCraft_MaterialsCost { get; set; }
    public int legendaryCraft_Time { get; set; }
}

[JsonObject]
public class HospitalLevelInfo : BuildingLevelInfo
{
    public int restoreHealthPerMinute { get; set; }
}

[JsonObject]
public class TyrLevelInfo : BuildingLevelInfo
{
    public int arrowsAmount { get; set; }
}

[JsonObject]
public class AlchemyLabLevelInfo : BuildingLevelInfo
{
    public int potionsInBattle { get; set; }
    public int craftCostInHerbs { get; set; }
    public int craftTime { get; set; }
}

[JsonObject]
public class ElixirWorkshopLevelInfo : BuildingLevelInfo
{
    public int skillLevelLimit { get; set; }
    public int elixirPriceInHerbs { get; set; }
}
