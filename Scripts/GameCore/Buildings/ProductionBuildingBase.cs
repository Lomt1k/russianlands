using System;
using System.Text;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public abstract class ProductionBuildingBase : BuildingBase
    {
        public abstract ResourceType resourceType { get; }

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
        protected abstract void SetStartFarmTime(ProfileBuildingsData data, long startFarmTime);

        public int GetCurrentLevelResourceLimit(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return 0;

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

        public int GetFarmedResourceAmount(ProfileBuildingsData data)
        {
            var startFarmTime = GetStartFarmTime(data);
            if (startFarmTime < 1) //fix incorrect startTime
            {
                SetStartFarmTime(data, DateTime.UtcNow.Ticks);
                return 0;
            }

            var farmHours = (DateTime.UtcNow - new DateTime(startFarmTime)).TotalHours;
            var farmPerHour = GetCurrentLevelFirstWorkerProductionPerHour(data) 
                + GetCurrentLevelSecondWorkerProductionPerHour(data);
            var farmedTotal = (int)(farmPerHour * farmHours);
            var farmLimit = GetCurrentLevelResourceLimit(data);

            return farmedTotal > farmLimit ? farmLimit : farmedTotal;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_production_per_hour_header"));
            var firstWorker = $"{Emojis.menuItems[MenuItem.Character]} {Localization.Get(session, $"building_{buildingType}_first_worker")}{Emojis.bigSpace}";
            sb.AppendLine(firstWorker + resourcePrefix + $" {GetCurrentLevelFirstWorkerProductionPerHour(data).View()}");
            var secondWorker = $"{Emojis.menuItems[MenuItem.Character]} {Localization.Get(session, $"building_{buildingType}_second_worker")}{Emojis.bigSpace}";
            sb.AppendLine(secondWorker + resourcePrefix + $" {GetCurrentLevelSecondWorkerProductionPerHour(data).View()}");

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_production_capacity_header"));
            var capacity = $"{resourcePrefix} {GetFarmedResourceAmount(data).View()} / {GetCurrentLevelResourceLimit(data).View()}";
            sb.Append(capacity);

            return sb.ToString();
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_production_per_hour_header"));

            bool hideDelta = !IsBuilt(data);
            var firstWorker = $"{Emojis.menuItems[MenuItem.Character]} {Localization.Get(session, $"building_{buildingType}_first_worker")}{Emojis.bigSpace}";
            var currentValue = GetCurrentLevelFirstWorkerProductionPerHour(data);
            var nextValue = GetNextLevelFirstWorkerProductionPerHour(data);
            var delta = nextValue - currentValue;
            sb.AppendLine(firstWorker + resourcePrefix + $" {nextValue.View()}" + (hideDelta ? string.Empty : $" (<i>+{delta.View()}</i>)"));

            var secondWorker = $"{Emojis.menuItems[MenuItem.Character]} {Localization.Get(session, $"building_{buildingType}_second_worker")}{Emojis.bigSpace}";
            currentValue = GetCurrentLevelSecondWorkerProductionPerHour(data);
            nextValue = GetNextLevelSecondWorkerProductionPerHour(data);
            delta = nextValue - currentValue;
            sb.AppendLine(secondWorker + resourcePrefix + $" {nextValue.View()}" + (hideDelta ? string.Empty : $" (<i>+{delta.View()}</i>)"));

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_production_capacity_header"));
            currentValue = GetCurrentLevelResourceLimit(data);
            nextValue = GetNextLevelResourceLimit(data);
            delta = nextValue - currentValue;
            var capacity = $"{resourcePrefix} {nextValue.View()}" + (hideDelta ? string.Empty : $" (<i>+{delta.View()}</i>)");
            sb.Append(capacity);

            return sb.ToString();
        }

    }
}
