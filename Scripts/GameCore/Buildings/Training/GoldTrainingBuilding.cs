using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings.Training
{
    public class GoldTrainingBuilding : TrainingBuildingBase
    {
        public override BuildingType buildingType => BuildingType.GoldTraining;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.goldTrainingLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.goldTrainingLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.goldTrainingStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.goldTrainingStartConstructionTime = startConstructionTime;
        }

        public override IEnumerable<sbyte> GetAllUnits(ProfileBuildingsData data)
        {
            if (BuildingType.GoldProductionFirst.GetBuilding().IsBuilt(data))
            {
                yield return 0;
                yield return 1;
            }
            if (BuildingType.GoldProductionSecond.GetBuilding().IsBuilt(data))
            {
                yield return 2;
                yield return 3;
            }
            if (BuildingType.GoldProductionThird.GetBuilding().IsBuilt(data))
            {
                yield return 4;
                yield return 5;
            }
        }

        public override string GetUnitName(GameSession session, ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingType = unitIndex < 2 ? BuildingType.GoldProductionFirst
                : unitIndex < 4 ? BuildingType.GoldProductionSecond
                : BuildingType.GoldProductionThird;

            var workerType = unitIndex % 2 == 0 ? "first" : "second";
            return Localization.Get(session, $"building_{buildingType}_{workerType}_worker");
        }

        public override string GetUnitIcon(ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingType = unitIndex < 2 ? BuildingType.GoldProductionFirst
                : unitIndex < 4 ? BuildingType.GoldProductionSecond
                : BuildingType.GoldProductionThird;

            var building = buildingType.GetBuilding() as ProductionBuildingBase;
            if (building == null)
                return string.Empty;

            bool isFirstWorker = unitIndex % 2 == 0;
            var charIcon = isFirstWorker ? building.firstWorkerIcon : building.secondWorkerIcon;
            return Emojis.characters[charIcon];
        }

        public override byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex)
        {
            var buildingType = unitIndex < 2 ? BuildingType.GoldProductionFirst
                : unitIndex < 4 ? BuildingType.GoldProductionSecond
                : BuildingType.GoldProductionThird;

            var building = buildingType.GetBuilding() as ProductionBuildingBase;
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

        public override long GetFirstTrainingUnitStartTime(ProfileBuildingsData data)
        {
            return data.goldTrainingFirstUnitStartTime;
        }

        public override void SetFirstTrainingUnitStartTime(ProfileBuildingsData data, long ticks)
        {
            data.goldTrainingFirstUnitStartTime = ticks;
        }

        public override sbyte GetSecondTrainingUnitIndex(ProfileBuildingsData data)
        {
            return data.goldTrainingSecondUnitIndex;
        }

        public override void SetSecondTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
        {
            data.goldTrainingSecondUnitIndex = unitIndex;
        }

        public override long GetSecondTrainingUnitStartTime(ProfileBuildingsData data)
        {
            return data.goldTrainingSecondUnitStartTime;
        }

        public override void SetSecondTrainingUnitStartTime(ProfileBuildingsData data, long ticks)
        {
            data.goldTrainingSecondUnitStartTime = ticks;
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
            var unitBuildingType = unitIndex < 2 ? BuildingType.GoldProductionFirst 
                : unitIndex < 4 ? BuildingType.GoldProductionSecond : BuildingType.GoldProductionThird;

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

    }
}
