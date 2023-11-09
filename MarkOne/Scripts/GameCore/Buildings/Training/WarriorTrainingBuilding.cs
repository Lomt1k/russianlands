using System;
using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Buildings.Training;

public class WarriorTrainingBuilding : TrainingBuildingBase
{
    public override BuildingId buildingId => BuildingId.WarriorTraining;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.warriorTrainingLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.warriorTrainingLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.warriorTrainingStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.warriorTrainingStartConstructionTime = startConstructionTime;
    }

    public override IEnumerable<sbyte> GetAllUnits(ProfileBuildingsData data)
    {
        yield return 0;
    }

    public override string GetUnitName(GameSession session, ProfileBuildingsData data, sbyte unitIndex)
    {
        return session.player.nickname;
    }

    public override string GetUnitIcon(ProfileBuildingsData data, sbyte unitIndex)
    {
        return data.session.player.avatarId.GetEmoji().ToString();
    }

    public override byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex)
    {
        return data.session.player.level;
    }

    public override sbyte GetFirstTrainingUnitIndex(ProfileBuildingsData data)
    {
        return data.warriorTrainingFirstUnitIndex;
    }

    public override void SetFirstTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
    {
        data.warriorTrainingFirstUnitIndex = unitIndex;
    }

    public override DateTime GetFirstTrainingUnitStartTime(ProfileBuildingsData data)
    {
        return data.warriorTrainingFirstUnitStartTime;
    }

    public override void SetFirstTrainingUnitStartTime(ProfileBuildingsData data, DateTime dateTime)
    {
        data.warriorTrainingFirstUnitStartTime = dateTime;
    }

    public override sbyte GetSecondTrainingUnitIndex(ProfileBuildingsData data)
    {
        return -1; //not used
    }

    public override void SetSecondTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
    {
        //not used
    }

    public override DateTime GetSecondTrainingUnitStartTime(ProfileBuildingsData data)
    {
        return DateTime.MinValue; //not used
    }

    public override void SetSecondTrainingUnitStartTime(ProfileBuildingsData data, DateTime dateTime)
    {
        //not used
    }

    public override void LevelUpFirst(GameSession session, ProfileBuildingsData data)
    {
        session.profile.data.level += 1;
        var playerStats = (PlayerStats)session.player.unitStats;
        playerStats.Recalculate();
        playerStats.SetFullHealth();

        SetFirstTrainingUnitIndex(data, -1);
        SetFirstTrainingUnitStartTime(data, DateTime.MinValue);
    }

    public override void LevelUpSecond(GameSession session, ProfileBuildingsData data)
    {
        //not used
    }

    public override int GetRequiredTrainingTime(byte currentLevel)
    {
        return ResourceHelper.GetWarriorTrainingTimeInSeconds(currentLevel);
    }

    public override string GetInfoAboutUnitTraining(GameSession session, ProfileBuildingsData data, sbyte unitIndex)
    {
        var sb = new StringBuilder();
        var currentHealth = session.player.unitStats.maxHP;
        var currentUnitLevel = GetUnitLevel(data, unitIndex);
        var maxUnitLevel = GetCurrentMaxUnitLevel(data);
        if (currentUnitLevel >= maxUnitLevel)
        {
            sb.AppendLine(Localization.Get(session, "building_training_max_health_header"));
            sb.Append(Emojis.StatHealth + currentHealth.View());
            return sb.ToString();
        }

        var bonusPerLevel = PlayerHealthByLevel.Get(currentUnitLevel + 1) - PlayerHealthByLevel.Get(currentUnitLevel);
        var nextHealth = currentHealth + bonusPerLevel;

        sb.AppendLine(Localization.Get(session, "building_training_max_health_header"));
        sb.Append(Emojis.StatHealth + nextHealth.View() + $" (<i>+{bonusPerLevel.View()}</i>)");
        return sb.ToString();
    }

}
