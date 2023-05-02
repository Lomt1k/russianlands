using System;
using System.Text;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.GameCore.Buildings;

public abstract class ProductionBuildingBase : BuildingBase
{
    public abstract ResourceId resourceId { get; }
    public abstract Avatar firstWorkerIcon { get; }
    public abstract Avatar secondWorkerIcon { get; }

    public string resourcePrefix;

    public ProductionBuildingBase()
    {
        resourcePrefix = resourceId.GetEmoji().ToString();
    }

    public abstract byte GetFirstWorkerLevel(ProfileBuildingsData data);
    public abstract byte GetSecondWorkerLevel(ProfileBuildingsData data);
    public abstract void SetFirstWorkerLevel(ProfileBuildingsData data, byte level);
    public abstract void SetSecondWorkerLevel(ProfileBuildingsData data, byte level);
    public abstract DateTime GetStartFarmTime(ProfileBuildingsData data);
    public abstract void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime);

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
        var productionPerHour = (GetFirstWorkerLevel(data) * levelInfo.bonusPerWorkerLevel) + basisPoint;
        return (int)productionPerHour;
    }

    public int GetCurrentLevelSecondWorkerProductionPerHour(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return 0;

        var levelInfo = (ProductionLevelInfo)buildingData.levels[currentLevel - 1];
        var basisPoint = (float)levelInfo.productionPerHour / 2;
        var productionPerHour = (GetSecondWorkerLevel(data) * levelInfo.bonusPerWorkerLevel) + basisPoint;
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
        var productionPerHour = (GetFirstWorkerLevel(data) * levelInfo.bonusPerWorkerLevel) + basisPoint;
        return (int)productionPerHour;
    }

    public int GetNextLevelSecondWorkerProductionPerHour(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        var levelInfo = (ProductionLevelInfo)buildingData.levels[currentLevel];
        var basisPoint = (float)levelInfo.productionPerHour / 2;
        var productionPerHour = (GetSecondWorkerLevel(data) * levelInfo.bonusPerWorkerLevel) + basisPoint;
        return (int)productionPerHour;
    }

    public int GetFarmedResourceAmount(ProfileBuildingsData data)
    {
        if (!IsBuilt(data))
            return 0;

        var startFarmTime = GetStartFarmTime(data);
        if (startFarmTime == DateTime.MinValue) //fix incorrect startTime
        {
            SetStartFarmTime(data, DateTime.UtcNow);
            return 0;
        }

        var dtNow = DateTime.UtcNow;
        var endFarmTimeDt = IsUnderConstruction(data) ? GetStartConstructionTime(data) : dtNow;

        var farmHours = (endFarmTimeDt - startFarmTime).TotalHours;
        var farmPerHour = GetCurrentLevelFirstWorkerProductionPerHour(data)
            + GetCurrentLevelSecondWorkerProductionPerHour(data);
        var farmedTotal = (int)(farmPerHour * farmHours);
        var farmLimit = GetCurrentLevelResourceLimit(data);

        if (IsUnderConstruction(data) && IsConstructionCanBeFinished(data))
        {
            // нужно посчитать то, что было добыто уже после завершения строительства
            startFarmTime = GetEndConstructionTime(data);
            endFarmTimeDt = dtNow;
            farmHours = (endFarmTimeDt - startFarmTime).TotalHours;
            farmPerHour = GetNextLevelFirstWorkerProductionPerHour(data)
            + GetNextLevelSecondWorkerProductionPerHour(data);
            farmedTotal += (int)(farmPerHour * farmHours);
            farmLimit = GetNextLevelResourceLimit(data);
        }

        return farmedTotal > farmLimit ? farmLimit : farmedTotal;
    }

    public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_production_per_hour_header"));
        if (IsUnderConstruction(data))
        {
            sb.AppendLine(Emojis.ElementConstruction + Localization.Get(session, "building_production_unavailable"));
        }
        else
        {
            var firstWorker = firstWorkerIcon.GetEmoji() + Localization.Get(session, $"building_{buildingId}_first_worker") + Emojis.bigSpace;
            sb.AppendLine(firstWorker + resourcePrefix + $" {GetCurrentLevelFirstWorkerProductionPerHour(data).View()}");
            var secondWorker = secondWorkerIcon.GetEmoji() + Localization.Get(session, $"building_{buildingId}_second_worker") + Emojis.bigSpace;
            sb.AppendLine(secondWorker + resourcePrefix + $" {GetCurrentLevelSecondWorkerProductionPerHour(data).View()}");
        }

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_production_capacity_header"));
        var capacity = $"{resourcePrefix} {GetFarmedResourceAmount(data).View()} / {GetCurrentLevelResourceLimit(data).View()}";
        sb.Append(capacity);

        return sb.ToString();
    }

    public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_production_per_hour_header"));

        var hideDelta = !IsBuilt(data);
        var firstWorker = firstWorkerIcon.GetEmoji() + Localization.Get(session, $"building_{buildingId}_first_worker") + Emojis.bigSpace;
        var currentValue = GetCurrentLevelFirstWorkerProductionPerHour(data);
        var nextValue = GetNextLevelFirstWorkerProductionPerHour(data);
        var delta = nextValue - currentValue;
        sb.AppendLine(firstWorker + resourcePrefix + $" {nextValue.View()}" + (hideDelta ? string.Empty : $" (<i>+{delta.View()}</i>)"));

        var secondWorker = secondWorkerIcon.GetEmoji() + Localization.Get(session, $"building_{buildingId}_second_worker") + Emojis.bigSpace;
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

    public override bool HasImportantUpdatesInternal(ProfileBuildingsData data)
    {
        var farmedAmount = GetFarmedResourceAmount(data);
        var limit = GetCurrentLevelResourceLimit(data);
        var isFarmLimitReached = farmedAmount >= limit;

        if (!isFarmLimitReached)
            return false;

        var isStorageLimitReached = data.session.player.resources.IsResourceLimitReached(resourceId);
        return !isStorageLimitReached;
    }

    protected override void OnConstructionStart(ProfileBuildingsData data)
    {
    }

    protected override void OnConstructionEnd(ProfileBuildingsData data, DateTime startConstructionTime, DateTime endConstructionTime)
    {
        /// Если до улучшения здания в нем хранилось 1500 золота, то после улучшения оно пересчитывается
        /// по новому уровню здания и становится 2000. Данный код исправляет эту проблему

        var targetFarmValue = GetFarmedResourceAmount(data);
        var newProductionPerHour = GetNextLevelFirstWorkerProductionPerHour(data) + GetNextLevelSecondWorkerProductionPerHour(data);
        var targetFarmHours = (double)targetFarmValue / newProductionPerHour;
        var startFarmTime = DateTime.UtcNow.AddHours(-targetFarmHours);
        SetStartFarmTime(data, startFarmTime);
    }

}
