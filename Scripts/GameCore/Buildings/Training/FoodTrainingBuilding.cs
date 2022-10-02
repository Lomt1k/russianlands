using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings.Training
{
    public class FoodTrainingBuilding : TrainingBuildingBase
    {
        public override BuildingType buildingType => BuildingType.FoodTraining;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.foodTrainingLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.foodTrainingLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.foodTrainingStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.foodTrainingStartConstructionTime = startConstructionTime;
        }

        public override IEnumerable<sbyte> GetAllUnits(ProfileBuildingsData data)
        {
            if (BuildingType.FoodProductionFirst.GetBuilding().IsBuilt(data))
            {
                yield return 0;
                yield return 1;
            }
            if (BuildingType.FoodProductionSecond.GetBuilding().IsBuilt(data))
            {
                yield return 2;
                yield return 3;
            }
            if (BuildingType.FoodProductionThird.GetBuilding().IsBuilt(data))
            {
                yield return 4;
                yield return 5;
            }
        }

        public override string GetUnitName(GameSession session, ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingType = unitIndex < 2 ? BuildingType.FoodProductionFirst
                : unitIndex < 4 ? BuildingType.FoodProductionSecond
                : BuildingType.FoodProductionThird;

            var workerType = unitIndex % 2 == 0 ? "first" : "second";
            return Localization.Get(session, $"building_{buildingType}_{workerType}_worker");
        }

        public override string GetUnitIcon(ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingType = unitIndex < 2 ? BuildingType.FoodProductionFirst
                : unitIndex < 4 ? BuildingType.FoodProductionSecond
                : BuildingType.FoodProductionThird;

            var building = buildingType.GetBuilding() as ProductionBuildingBase;
            if (building == null)
                return string.Empty;

            bool isFirstWorker = unitIndex % 2 == 0;
            var charIcon = isFirstWorker ? building.firstWorkerIcon : building.secondWorkerIcon;
            return Emojis.characters[charIcon];
        }

        public override byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingType = unitIndex < 2 ? BuildingType.FoodProductionFirst
                : unitIndex < 4 ? BuildingType.FoodProductionSecond
                : BuildingType.FoodProductionThird;

            var building = buildingType.GetBuilding() as ProductionBuildingBase;
            if (building == null)
                return 0;

            bool isFirstWorker = unitIndex % 2 == 0;
            return isFirstWorker ? building.GetFirstWorkerLevel(data) : building.GetSecondWorkerLevel(data);
        }

        public override sbyte GetFirstTrainingUnitIndex(ProfileBuildingsData data)
        {
            return data.foodTrainingFirstUnitIndex;
        }

        public override void SetFirstTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
        {
            data.foodTrainingFirstUnitIndex = unitIndex;
        }

        public override long GetFirstTrainingUnitStartTime(ProfileBuildingsData data)
        {
            return data.foodTrainingFirstUnitStartTime;
        }

        public override void SetFirstTrainingUnitStartTime(ProfileBuildingsData data, long ticks)
        {
            data.foodTrainingFirstUnitStartTime = ticks;
        }

        public override sbyte GetSecondTrainingUnitIndex(ProfileBuildingsData data)
        {
            return data.foodTrainingSecondUnitIndex;
        }

        public override void SetSecondTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
        {
            data.foodTrainingSecondUnitIndex = unitIndex;
        }

        public override long GetSecondTrainingUnitStartTime(ProfileBuildingsData data)
        {
            return data.foodTrainingSecondUnitStartTime;
        }

        public override void SetSecondTrainingUnitStartTime(ProfileBuildingsData data, long ticks)
        {
            data.foodTrainingSecondUnitStartTime = ticks;
        }

        public override void LevelUpFirst(GameSession session, ProfileBuildingsData data)
        {
            var unitIndex = GetFirstTrainingUnitIndex(data);
            LevelUpUnitByIndex(data, unitIndex);
            SetFirstTrainingUnitIndex(data, -1);
            SetFirstTrainingUnitStartTime(data, 0);
        }

        public override void LevelUpSecond(GameSession session, ProfileBuildingsData data)
        {
            var unitIndex = GetSecondTrainingUnitIndex(data);
            LevelUpUnitByIndex(data, unitIndex);
            SetSecondTrainingUnitIndex(data, -1);
            SetSecondTrainingUnitStartTime(data, 0);
        }

        private void LevelUpUnitByIndex(ProfileBuildingsData data, sbyte unitIndex)
        {
            var unitBuildingType = unitIndex < 2 ? BuildingType.FoodProductionFirst
                : unitIndex < 4 ? BuildingType.FoodProductionSecond : BuildingType.FoodProductionThird;

            var productionBuilding = (ProductionBuildingBase)unitBuildingType.GetBuilding();
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
            var unitBuildingType = unitIndex < 2 ? BuildingType.FoodProductionFirst
                : unitIndex < 4 ? BuildingType.FoodProductionSecond : BuildingType.FoodProductionThird;

            var productionBuilding = (ProductionBuildingBase)unitBuildingType.GetBuilding();
            bool isFirstUnit = unitIndex % 2 == 0;
            var currentProduction = isFirstUnit
                ? productionBuilding.GetCurrentLevelFirstWorkerProductionPerHour(data)
                : productionBuilding.GetCurrentLevelSecondWorkerProductionPerHour(data);

            var currentBuildingLevel = productionBuilding.GetCurrentLevel(data);
            var currentBuildingLevelInfo = (ProductionLevelInfo)productionBuilding.buildingData.levels[currentBuildingLevel - 1];
            var bonusPerLevel = (int)Math.Round(currentBuildingLevelInfo.bonusPerWorkerLevel);
            var nextProduction = currentProduction + bonusPerLevel;

            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "building_production_per_hour_header"));
            sb.Append(productionBuilding.resourcePrefix + $" {nextProduction.View()} (<i>+{bonusPerLevel.View()}</i>)");
            return sb.ToString();
        }

    }
}
