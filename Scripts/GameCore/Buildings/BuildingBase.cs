using System;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public abstract class BuildingBase
    {
        public abstract BuildingData buildingData { get; }

        public abstract int GetCurrentLevel(ProfileBuildingsData data);
        protected abstract long GetStartConstructionTime(ProfileBuildingsData data);

        /// <returns>Построено ли здание (хотя бы 1-ый уровень)</returns>
        public bool IsBuilt(ProfileBuildingsData data)
        {
            return GetCurrentLevel(data) > 0;
        }

        public bool IsMaxLevel(ProfileBuildingsData data)
        {
            return GetCurrentLevel(data) >= buildingData.levels.Count;
        }

        public bool IsUnderConstruction(ProfileBuildingsData data)
        {
            return GetStartConstructionTime(data) > 0;
        }

        public DateTime GetEndConstructionTime(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            var ticks = GetStartConstructionTime(data);
            var startDt = new DateTime(ticks);
            var secondsForConstruction = buildingData.levels[currentLevel].constructionTime;
            var endDt = startDt.AddSeconds(secondsForConstruction);
            return endDt;
        }

        public bool IsConstructionCanBeFinished(ProfileBuildingsData data)
        {
            return DateTime.UtcNow > GetEndConstructionTime(data);
        }

    }
}
