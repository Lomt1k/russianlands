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
    public class HerbsTrainingBuilding : TrainingBuildingBase
    {
        public override BuildingType buildingType => BuildingType.HerbsTraining;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.herbsTrainingLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsTrainingLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.herbsTrainingStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.herbsTrainingStartConstructionTime = startConstructionTime;
        }

        public override IEnumerable<sbyte> GetAllUnits(ProfileBuildingsData data)
        {
            if (BuildingType.HerbsProductionFirst.GetBuilding().IsBuilt(data))
            {
                yield return 0;
                yield return 1;
            }
            if (BuildingType.HerbsProductionSecond.GetBuilding().IsBuilt(data))
            {
                yield return 2;
                yield return 3;
            }
            if (BuildingType.HerbsProductionThird.GetBuilding().IsBuilt(data))
            {
                yield return 4;
                yield return 5;
            }
        }

        public override string GetUnitName(GameSession session, ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingType = unitIndex < 2 ? BuildingType.HerbsProductionFirst
                : unitIndex < 4 ? BuildingType.HerbsProductionSecond
                : BuildingType.HerbsProductionThird;

            var workerType = unitIndex % 2 == 0 ? "first" : "second";
            return Localization.Get(session, $"building_{buildingType}_{workerType}_worker");
        }

        public override string GetUnitIcon(ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingType = unitIndex < 2 ? BuildingType.HerbsProductionFirst
                : unitIndex < 4 ? BuildingType.HerbsProductionSecond
                : BuildingType.HerbsProductionThird;

            var building = buildingType.GetBuilding() as ProductionBuildingBase;
            if (building == null)
                return string.Empty;

            bool isFirstWorker = unitIndex % 2 == 0;
            var charIcon = isFirstWorker ? building.firstWorkerIcon : building.secondWorkerIcon;
            return Emojis.characters[charIcon];
        }

        public override byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingType = unitIndex < 2 ? BuildingType.HerbsProductionFirst
                : unitIndex < 4 ? BuildingType.HerbsProductionSecond
                : BuildingType.HerbsProductionThird;

            var building = buildingType.GetBuilding() as ProductionBuildingBase;
            if (building == null)
                return 0;

            bool isFirstWorker = unitIndex % 2 == 0;
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

        public override long GetFirstTrainingUnitStartTime(ProfileBuildingsData data)
        {
            return data.herbsTrainingFirstUnitStartTime;
        }

        public override void SetFirstTrainingUnitStartTime(ProfileBuildingsData data, long ticks)
        {
            data.herbsTrainingFirstUnitStartTime = ticks;
        }

        public override sbyte GetSecondTrainingUnitIndex(ProfileBuildingsData data)
        {
            return data.herbsTrainingSecondUnitIndex;
        }

        public override void SetSecondTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
        {
            data.herbsTrainingSecondUnitIndex = unitIndex;
        }

        public override long GetSecondTrainingUnitStartTime(ProfileBuildingsData data)
        {
            return data.herbsTrainingSecondUnitStartTime;
        }

        public override void SetSecondTrainingUnitStartTime(ProfileBuildingsData data, long ticks)
        {
            data.herbsTrainingSecondUnitStartTime = ticks;
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
            var unitBuildingType = unitIndex < 2 ? BuildingType.HerbsProductionFirst
                : unitIndex < 4 ? BuildingType.HerbsProductionSecond : BuildingType.HerbsProductionThird;

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
            var unitBuildingType = unitIndex < 2 ? BuildingType.HerbsProductionFirst
                : unitIndex < 4 ? BuildingType.HerbsProductionSecond : BuildingType.HerbsProductionThird;

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
