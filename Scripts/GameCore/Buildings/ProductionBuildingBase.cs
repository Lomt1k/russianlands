using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public abstract class ProductionBuildingBase : BuildingBase
    {
        public abstract ResourceType resourceType { get; }
        public abstract int resourceLimitForZeroLevel { get; }

        public string resourcePrefix;

        public ProductionBuildingBase()
        {
            resourcePrefix = Emojis.resources[resourceType];
        }

        public abstract byte GetFirstWorkerLevel(ProfileBuildingsData data);
        public abstract byte GetSecondWorkerLevel(ProfileBuildingsData data);
        public abstract void SetFirstWorkerLevel(ProfileBuildingsData data, byte level);
        public abstract void SetSecondWorkerLevel(ProfileBuildingsData data, byte level);
        protected abstract long GetStartFarmTime(ProfileBuildingsData data);
        protected abstract void SetStartFarmTime(ProfileBuildingsData data, long startConstructionTime);

        public int GetCurrentLevelResourceLimit(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return resourceLimitForZeroLevel;

            var levelInfo = (ProductionLevelInfo)buildingData.levels[currentLevel - 1];
            return levelInfo.resourceStorageLimit;
        }

        public int GetCurrentLevelFirstWorkerProductionPerHour(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return 0;

            var levelInfo = (ProductionLevelInfo)buildingData.levels[currentLevel - 1];
            var basisPoint = (float)levelInfo.productionPerHour / 2;
            var productionPerHour = GetFirstWorkerLevel(data) * levelInfo.bonusPerWorkerLevel + basisPoint;
            return (int)productionPerHour;
        }

        public int GetCurrentLevelSecondWorkerProductionPerHour(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return 0;

            var levelInfo = (ProductionLevelInfo)buildingData.levels[currentLevel - 1];
            var basisPoint = (float)levelInfo.productionPerHour / 2;
            var productionPerHour = GetSecondWorkerLevel(data) * levelInfo.bonusPerWorkerLevel + basisPoint;
            return (int)productionPerHour;
        }

        public int GetNextLevelResourceLimit(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            var levelInfo = (ProductionLevelInfo)buildingData.levels[currentLevel];
            return levelInfo.resourceStorageLimit;
        }

        public int GetNextLevelFirstWorkerProductionPerHour(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            var levelInfo = (ProductionLevelInfo)buildingData.levels[currentLevel];
            var basisPoint = (float)levelInfo.productionPerHour / 2;
            var productionPerHour = GetFirstWorkerLevel(data) * levelInfo.bonusPerWorkerLevel + basisPoint;
            return (int)productionPerHour;
        }

        public int GetNextLevelSecondWorkerProductionPerHour(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            var levelInfo = (ProductionLevelInfo)buildingData.levels[currentLevel];
            var basisPoint = (float)levelInfo.productionPerHour / 2;
            var productionPerHour = GetSecondWorkerLevel(data) * levelInfo.bonusPerWorkerLevel + basisPoint;
            return (int)productionPerHour;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            //var sb = new StringBuilder();
            //sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));

            //sb.AppendLine();
            //sb.AppendLine(Localization.Get(session, "building_storage_capacity_header"));
            //var playerResources = session.player.resources;
            //var currentValue = playerResources.GetValue(resourceType).View();
            //var limitValue = GetCurrentLevelResourceLimit(data).View();
            //var capacity = $"{resourcePrefix} {currentValue} / {limitValue}";
            //sb.Append(capacity);

            //return sb.ToString();
            return string.Empty;
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            //var sb = new StringBuilder();
            //sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));

            //sb.AppendLine();
            //sb.AppendLine(Localization.Get(session, "building_storage_capacity_header"));
            //var currentLimit = GetCurrentLevelResourceLimit(data);
            //var nextLimit = GetNextLevelResourceLimit(data);
            //var delta = nextLimit - currentLimit;
            //var capacity = $"{resourcePrefix} {nextLimit.View()} (<i>+{delta.View()}</i>)";
            //sb.Append(capacity);

            //return sb.ToString();
            return string.Empty;
        }
    }
}
