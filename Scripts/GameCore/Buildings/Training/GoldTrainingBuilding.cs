using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.GameCore.Buildings.Training
{
    public class GoldTrainingBuilding : TrainingBuildingBase
    {
        public override BuildingId buildingId => BuildingId.GoldTraining;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.goldTrainingLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.goldTrainingLevel = level;
        }

        protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.goldTrainingStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
        {
            data.goldTrainingStartConstructionTime = startConstructionTime;
        }

        public override IEnumerable<sbyte> GetAllUnits(ProfileBuildingsData data)
        {
            if (BuildingId.GoldProductionFirst.GetBuilding().IsBuilt(data))
            {
                yield return 0;
                yield return 1;
            }
            if (BuildingId.GoldProductionSecond.GetBuilding().IsBuilt(data))
            {
                yield return 2;
                yield return 3;
            }
            if (BuildingId.GoldProductionThird.GetBuilding().IsBuilt(data))
            {
                yield return 4;
                yield return 5;
            }
        }

        public override string GetUnitName(GameSession session, ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingId = unitIndex < 2 ? BuildingId.GoldProductionFirst
                : unitIndex < 4 ? BuildingId.GoldProductionSecond
                : BuildingId.GoldProductionThird;

            var workerType = unitIndex % 2 == 0 ? "first" : "second";
            return Localization.Get(session, $"building_{buildingId}_{workerType}_worker");
        }

        public override string GetUnitIcon(ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingId = unitIndex < 2 ? BuildingId.GoldProductionFirst
                : unitIndex < 4 ? BuildingId.GoldProductionSecond
                : BuildingId.GoldProductionThird;

            var building = buildingId.GetBuilding() as ProductionBuildingBase;
            if (building == null)
                return string.Empty;

            bool isFirstWorker = unitIndex % 2 == 0;
            var avatar = isFirstWorker ? building.firstWorkerIcon : building.secondWorkerIcon;
            return avatar.GetEmoji().ToString();
        }

        public override byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingId = unitIndex < 2 ? BuildingId.GoldProductionFirst
                : unitIndex < 4 ? BuildingId.GoldProductionSecond
                : BuildingId.GoldProductionThird;

            var building = buildingId.GetBuilding() as ProductionBuildingBase;
            if (building == null)
                return 0;

            bool isFirstWorker = unitIndex % 2 == 0;
            return isFirstWorker ? building.GetFirstWorkerLevel(data) : building.GetSecondWorkerLevel(data);
        }

        public override sbyte GetFirstTrainingUnitIndex(ProfileBuildingsData data)
        {
            return data.goldTrainingFirstUnitIndex;
        }

        public override void SetFirstTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
        {
            data.goldTrainingFirstUnitIndex = unitIndex;
        }

        public override DateTime GetFirstTrainingUnitStartTime(ProfileBuildingsData data)
        {
            return data.goldTrainingFirstUnitStartTime;
        }

        public override void SetFirstTrainingUnitStartTime(ProfileBuildingsData data, DateTime dateTime)
        {
            data.goldTrainingFirstUnitStartTime = dateTime;
        }

        public override sbyte GetSecondTrainingUnitIndex(ProfileBuildingsData data)
        {
            return data.goldTrainingSecondUnitIndex;
        }

        public override void SetSecondTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
        {
            data.goldTrainingSecondUnitIndex = unitIndex;
        }

        public override DateTime GetSecondTrainingUnitStartTime(ProfileBuildingsData data)
        {
            return data.goldTrainingSecondUnitStartTime;
        }

        public override void SetSecondTrainingUnitStartTime(ProfileBuildingsData data, DateTime dateTime)
        {
            data.goldTrainingSecondUnitStartTime = dateTime;
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
            var unitBuildingId = unitIndex < 2 ? BuildingId.GoldProductionFirst 
                : unitIndex < 4 ? BuildingId.GoldProductionSecond : BuildingId.GoldProductionThird;

            var productionBuilding = (ProductionBuildingBase)unitBuildingId.GetBuilding();
            bool isFirstUnit = unitIndex % 2 == 0;

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
            var unitBuildingId = unitIndex < 2 ? BuildingId.GoldProductionFirst
                : unitIndex < 4 ? BuildingId.GoldProductionSecond : BuildingId.GoldProductionThird;

            var productionBuilding = (ProductionBuildingBase)unitBuildingId.GetBuilding();
            bool isFirstUnit = unitIndex % 2 == 0;
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
}
