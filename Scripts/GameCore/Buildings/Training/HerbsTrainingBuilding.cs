﻿using System;
using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Buildings.Training;

public class HerbsTrainingBuilding : TrainingBuildingBase
{
    public override BuildingId buildingId => BuildingId.HerbsTraining;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.herbsTrainingLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.herbsTrainingLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.herbsTrainingStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.herbsTrainingStartConstructionTime = startConstructionTime;
    }

    public override IEnumerable<sbyte> GetAllUnits(ProfileBuildingsData data)
    {
        if (BuildingId.HerbsProductionFirst.GetBuilding().IsBuilt(data))
        {
            yield return 0;
            yield return 1;
        }
        if (BuildingId.HerbsProductionSecond.GetBuilding().IsBuilt(data))
        {
            yield return 2;
            yield return 3;
        }
        if (BuildingId.HerbsProductionThird.GetBuilding().IsBuilt(data))
        {
            yield return 4;
            yield return 5;
        }
    }

    public override string GetUnitName(GameSession session, ProfileBuildingsData data, sbyte unitIndex)
    {
        var buildingId = unitIndex < 2 ? BuildingId.HerbsProductionFirst
            : unitIndex < 4 ? BuildingId.HerbsProductionSecond
            : BuildingId.HerbsProductionThird;

        var workerType = unitIndex % 2 == 0 ? "first" : "second";
        return Localization.Get(session, $"building_{buildingId}_{workerType}_worker");
    }

    public override string GetUnitIcon(ProfileBuildingsData data, sbyte unitIndex)
    {
        var buildingId = unitIndex < 2 ? BuildingId.HerbsProductionFirst
            : unitIndex < 4 ? BuildingId.HerbsProductionSecond
            : BuildingId.HerbsProductionThird;

        var building = buildingId.GetBuilding() as ProductionBuildingBase;
        if (building == null)
            return string.Empty;

        var isFirstWorker = unitIndex % 2 == 0;
        var avatar = isFirstWorker ? building.firstWorkerIcon : building.secondWorkerIcon;
        return avatar.GetEmoji().ToString();
    }

    public override byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex)
    {
        var buildingId = unitIndex < 2 ? BuildingId.HerbsProductionFirst
            : unitIndex < 4 ? BuildingId.HerbsProductionSecond
            : BuildingId.HerbsProductionThird;

        var building = buildingId.GetBuilding() as ProductionBuildingBase;
        if (building == null)
            return 0;

        var isFirstWorker = unitIndex % 2 == 0;
        return isFirstWorker ? building.GetFirstWorkerLevel(data) : building.GetSecondWorkerLevel(data);
    }

    public override sbyte GetFirstTrainingUnitIndex(ProfileBuildingsData data)
    {
        return data.herbsTrainingFirstUnitIndex;
    }

    public override void SetFirstTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
    {
        data.herbsTrainingFirstUnitIndex = unitIndex;
    }

    public override DateTime GetFirstTrainingUnitStartTime(ProfileBuildingsData data)
    {
        return data.herbsTrainingFirstUnitStartTime;
    }

    public override void SetFirstTrainingUnitStartTime(ProfileBuildingsData data, DateTime dateTime)
    {
        data.herbsTrainingFirstUnitStartTime = dateTime;
    }

    public override sbyte GetSecondTrainingUnitIndex(ProfileBuildingsData data)
    {
        return data.herbsTrainingSecondUnitIndex;
    }

    public override void SetSecondTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
    {
        data.herbsTrainingSecondUnitIndex = unitIndex;
    }

    public override DateTime GetSecondTrainingUnitStartTime(ProfileBuildingsData data)
    {
        return data.herbsTrainingSecondUnitStartTime;
    }

    public override void SetSecondTrainingUnitStartTime(ProfileBuildingsData data, DateTime dateTime)
    {
        data.herbsTrainingSecondUnitStartTime = dateTime;
    }

    public override void LevelUpFirst(GameSession session, ProfileBuildingsData data)
    {
        var unitIndex = GetFirstTrainingUnitIndex(data);
        LevelUpUnitByIndex(data, unitIndex);
        SetFirstTrainingUnitIndex(data, -1);
        SetFirstTrainingUnitStartTime(data, DateTime.MinValue);
    }

    public override void LevelUpSecond(GameSession session, ProfileBuildingsData data)
    {
        var unitIndex = GetSecondTrainingUnitIndex(data);
        LevelUpUnitByIndex(data, unitIndex);
        SetSecondTrainingUnitIndex(data, -1);
        SetSecondTrainingUnitStartTime(data, DateTime.MinValue);
    }

    private void LevelUpUnitByIndex(ProfileBuildingsData data, sbyte unitIndex)
    {
        var unitBuildingId = unitIndex < 2 ? BuildingId.HerbsProductionFirst
            : unitIndex < 4 ? BuildingId.HerbsProductionSecond : BuildingId.HerbsProductionThird;

        var productionBuilding = (ProductionBuildingBase)unitBuildingId.GetBuilding();
        var isFirstUnit = unitIndex % 2 == 0;

        if (isFirstUnit)
        {
            var currentLevel = productionBuilding.GetFirstWorkerLevel(data);
            productionBuilding.SetFirstWorkerLevel(data, (byte)(currentLevel + 1));
        }
        else
        {
            var currentLevel = productionBuilding.GetSecondWorkerLevel(data);
            productionBuilding.SetSecondWorkerLevel(data, (byte)(currentLevel + 1));
        }
    }

    public override int GetRequiredTrainingTime(byte currentLevel)
    {
        return ResourceHelper.GetDefaultResourceTrainingTimeInSeconds(currentLevel);
    }

    public override string GetInfoAboutUnitTraining(GameSession session, ProfileBuildingsData data, sbyte unitIndex)
    {
        var sb = new StringBuilder();
        var unitBuildingId = unitIndex < 2 ? BuildingId.HerbsProductionFirst
            : unitIndex < 4 ? BuildingId.HerbsProductionSecond : BuildingId.HerbsProductionThird;

        var productionBuilding = (ProductionBuildingBase)unitBuildingId.GetBuilding();
        var isFirstUnit = unitIndex % 2 == 0;
        var currentProduction = isFirstUnit
            ? productionBuilding.GetCurrentLevelFirstWorkerProductionPerHour(data)
            : productionBuilding.GetCurrentLevelSecondWorkerProductionPerHour(data);

        var currentUnitLevel = GetUnitLevel(data, unitIndex);
        var maxUnitLevel = GetCurrentMaxUnitLevel(data);
        if (currentUnitLevel >= maxUnitLevel)
        {
            sb.AppendLine(Localization.Get(session, "building_production_per_hour_header"));
            sb.Append(productionBuilding.resourcePrefix + $" {currentProduction.View()}");
            return sb.ToString();
        }

        var currentBuildingLevel = productionBuilding.GetCurrentLevel(data);
        var currentBuildingLevelInfo = (ProductionLevelInfo)productionBuilding.buildingData.levels[currentBuildingLevel - 1];
        var bonusPerLevel = (int)Math.Round(currentBuildingLevelInfo.bonusPerWorkerLevel);
        var nextProduction = currentProduction + bonusPerLevel;

        sb.AppendLine(Localization.Get(session, "building_production_per_hour_header"));
        sb.Append(productionBuilding.resourcePrefix + $" {nextProduction.View()} (<i>+{bonusPerLevel.View()}</i>)");
        return sb.ToString();
    }

}
