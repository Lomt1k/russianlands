using System.Text;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public abstract class StorageBuildingBase : BuildingBase
    {
        public abstract ResourceType resourceType { get; }
        public abstract int resourceLimitForZeroLevel { get; }

        public string resourcePrefix;

        public StorageBuildingBase()
        {
            resourcePrefix = resourceType.GetEmoji().ToString();
        }

        public int GetCurrentLevelResourceLimit(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return resourceLimitForZeroLevel;

            var levelInfo = (StorageLevelInfo)buildingData.levels[currentLevel - 1];
            return levelInfo.resourceStorageLimit;
        }

        public int GetNextLevelResourceLimit(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            var levelInfo = (StorageLevelInfo)buildingData.levels[currentLevel];
            return levelInfo.resourceStorageLimit;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_storage_capacity_header"));
            var playerResources = session.player.resources;
            var currentValue = playerResources.GetValue(resourceType).View();
            var limitValue = GetCurrentLevelResourceLimit(data).View();
            var capacity = $"{resourcePrefix} {currentValue} / {limitValue}";
            sb.Append(capacity);

            return sb.ToString();
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_storage_capacity_header"));
            var currentLimit = GetCurrentLevelResourceLimit(data);
            var nextLimit = GetNextLevelResourceLimit(data);
            var delta = nextLimit - currentLimit;
            var capacity = $"{resourcePrefix} {nextLimit.View()} (<i>+{delta.View()}</i>)";
            sb.Append(capacity);

            return sb.ToString();
        }

    }
}
