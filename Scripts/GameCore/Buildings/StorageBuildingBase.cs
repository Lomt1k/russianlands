using System.Text;
using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Buildings;

public abstract class StorageBuildingBase : BuildingBase
{
    public abstract ResourceId resourceId { get; }
    public abstract int resourceLimitForZeroLevel { get; }

    public string resourcePrefix;

    public StorageBuildingBase()
    {
        resourcePrefix = resourceId.GetEmoji().ToString();
    }

    public int GetCurrentLevelResourceLimit(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return resourceLimitForZeroLevel;

        var levelInfo = (StorageLevelInfo)buildingData.levels[currentLevel - 1];
        return levelInfo.resourceStorageLimit;
    }

    public int GetNextLevelResourceLimit(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        var levelInfo = (StorageLevelInfo)buildingData.levels[currentLevel];
        return levelInfo.resourceStorageLimit;
    }

    public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_storage_capacity_header"));
        var playerResources = session.player.resources;
        var currentValue = playerResources.GetValue(resourceId).View();
        var limitValue = GetCurrentLevelResourceLimit(data).View();
        var capacity = $"{resourcePrefix} {currentValue} / {limitValue}";
        sb.Append(capacity);

        return sb.ToString();
    }

    public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_storage_capacity_header"));
        var currentLimit = GetCurrentLevelResourceLimit(data);
        var nextLimit = GetNextLevelResourceLimit(data);
        var delta = nextLimit - currentLimit;
        var capacity = $"{resourcePrefix} {nextLimit.View()} (<i>+{delta.View()}</i>)";
        sb.Append(capacity);

        return sb.ToString();
    }

}
