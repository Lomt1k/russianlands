using System;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Buildings.General;

public class TyrBuilding : BuildingBase
{
    public override BuildingId buildingId => BuildingId.Tyr;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.tyrLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.tyrLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.tyrStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.tyrStartConstructionTime = startConstructionTime;
    }

    public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));

        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return sb.ToString();

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_tyr_arrows_header"));
        var levelInfo = (TyrLevelInfo)buildingData.levels[currentLevel - 1];
        var arrowsAmount = levelInfo.arrowsAmount;
        sb.Append(Emojis.StatArrows + arrowsAmount.ToString());

        return sb.ToString();
    }

    public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));

        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return sb.ToString();

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_tyr_arrows_header"));
        var currentLevelInfo = (TyrLevelInfo)buildingData.levels[currentLevel - 1];
        var currentArrowsAmount = currentLevelInfo.arrowsAmount;
        var nextLevelInfo = (TyrLevelInfo)buildingData.levels[currentLevel];
        var nextArrowsAmount = nextLevelInfo.arrowsAmount;
        var delta = nextArrowsAmount - currentArrowsAmount;
        sb.Append(Emojis.StatArrows + nextArrowsAmount.ToString() + $" (<i>+{delta}</i>)");

        return sb.ToString();
    }

    public byte GetArrowsAmount(GameSession session, ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return 0;

        var currentLevelInfo = (TyrLevelInfo)buildingData.levels[currentLevel - 1];
        return (byte)currentLevelInfo.arrowsAmount;
    }

}
