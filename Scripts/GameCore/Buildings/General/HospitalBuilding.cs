using System;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Buildings.General;

public class HospitalBuilding : BuildingBase
{
    public override BuildingId buildingId => BuildingId.Hospital;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.hospitalLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.hospitalLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.hospitalStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.hospitalStartConstructionTime = startConstructionTime;
    }

    public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));

        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return sb.ToString();

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_hospital_health_per_second_header"));
        var levelInfo = (HospitalLevelInfo)buildingData.levels[currentLevel - 1];
        var healthAmount = levelInfo.restoreHealthPerMinute;
        sb.Append(Emojis.StatHealth + healthAmount.ToString());

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
        sb.AppendLine(Localization.Get(session, "building_hospital_health_per_second_header"));
        var currentLevelInfo = (HospitalLevelInfo)buildingData.levels[currentLevel - 1];
        var currentHealthAmount = currentLevelInfo.restoreHealthPerMinute;
        var nextLevelInfo = (HospitalLevelInfo)buildingData.levels[currentLevel];
        var nextHealthAmount = nextLevelInfo.restoreHealthPerMinute;
        var delta = nextHealthAmount - currentHealthAmount;
        sb.Append(Emojis.StatHealth + nextHealthAmount.ToString() + $" (<i>+{delta}</i>)");

        return sb.ToString();
    }

    public int GetHealthRestorePerMinute(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return 0;

        var levelInfo = (HospitalLevelInfo)buildingData.levels[currentLevel - 1];
        return levelInfo.restoreHealthPerMinute;
    }

    public void SetLastRegenTimeAsNow(ProfileBuildingsData data)
    {
        data.hospitalLastHealthRestoreTime = DateTime.UtcNow;
    }

    public DateTime GetLastRegenTime(ProfileBuildingsData data)
    {
        return data.hospitalLastHealthRestoreTime;
    }

}
