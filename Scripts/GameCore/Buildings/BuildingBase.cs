using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public abstract class BuildingBase
    {
        public abstract BuildingType buildingType { get; }
        public BuildingData buildingData => GameDataBase.GameDataBase.instance.buildings[(int)buildingType];

        public abstract byte GetCurrentLevel(ProfileBuildingsData data);
        public abstract string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data);
        public abstract string GetNextLevelInfo(GameSession session, ProfileBuildingsData data);
        protected abstract void SetCurrentLevel(ProfileBuildingsData data, byte level);
        protected abstract long GetStartConstructionTime(ProfileBuildingsData data);
        protected abstract void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime);

        public bool HasImportantUpdates(ProfileBuildingsData data)
        {
            if (!IsBuilt(data))
                return false;

            if (IsUnderConstruction(data) && IsConstructionCanBeFinished(data))
            {
                return true;
            }

            return HasImportantUpdatesInternal(data);
        }

        public virtual bool HasImportantUpdatesInternal(ProfileBuildingsData data)
        {
            return false;
        }

        public List<string> GetUpdates(GameSession session, ProfileBuildingsData data)
        {
            var updates = new List<string>();

            if (!IsBuilt(data))
                return updates;

            if (IsUnderConstruction(data))
            {
                if (IsConstructionCanBeFinished(data))
                {
                    LevelUp(data);
                    var currentLevel = GetCurrentLevel(data);
                    var update = currentLevel > 1
                        ? string.Format(Localization.Get(session, "building_construction_end"), currentLevel)
                        : Localization.Get(session, "building_build_end");
                    updates.Add(update);
                }
                else
                {
                    var endTime = GetEndConstructionTime(data);
                    var timeToEnd = endTime - DateTime.UtcNow;
                    string key = IsBuilt(data) ? "construction" : "build";
                    var update = string.Format(Localization.Get(session, $"building_{key}_progress"), timeToEnd.GetView(session));
                    updates.Add(update);
                }
            }

            var internalUpdates = GetUpdatesInternal(session, data);
            updates.AddRange(internalUpdates);
            return updates;
        }

        protected virtual List<string> GetUpdatesInternal(GameSession session, ProfileBuildingsData data)
        {
            return new List<string>();
        }

        public string GetLocalizedName(GameSession session, ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            return Localization.Get(session, "building_name_" + buildingType.ToString())
                + (currentLevel > 0 ? string.Format(Localization.Get(session, "building_level_suffix"), currentLevel) : string.Empty );
        }

        public string GetNextLevelLocalizedName(GameSession session, ProfileBuildingsData data)
        {
            if (IsMaxLevel(data))
                return string.Empty;

            var nextLevel = GetCurrentLevel(data) + 1;
            return Localization.Get(session, "building_name_" + buildingType.ToString())
                + (nextLevel > 0 ? string.Format(Localization.Get(session, "building_level_suffix"), nextLevel) : string.Empty);
        }


        /// <returns>Построено ли здание (хотя бы 1-ый уровень)</returns>
        public bool IsBuilt(ProfileBuildingsData data)
        {
            return GetCurrentLevel(data) > 0;
        }

        /// <returns>Достигло ли здание максимального уровня</returns>
        public bool IsMaxLevel(ProfileBuildingsData data)
        {
            return GetCurrentLevel(data) >= buildingData.levels.Count;
        }

        /// <returns>Находится ли здание в процессе постройки / улучшения</returns>
        public bool IsUnderConstruction(ProfileBuildingsData data)
        {
            return GetStartConstructionTime(data) > 0;
        }

        /// <summary>
        /// Запускает процесс постройки / улучшения здания
        /// </summary>
        public void StartConstruction(ProfileBuildingsData data)
        {
            if (IsNextLevelUnlocked(data))
            {
                SetStartConstructionTime(data, DateTime.UtcNow.Ticks);
            }
        }

        /// <returns>Доступен ли следующий уровень на текущем уровне ратуши</returns>
        public bool IsNextLevelUnlocked(ProfileBuildingsData data)
        {
            if (IsMaxLevel(data))
                return false;

            var currentLevel = GetCurrentLevel(data);
            var nextLevel = buildingData.levels[currentLevel];
            return data.townHallLevel >= nextLevel.requiredTownHall;
        }

        /// <returns>Дата, когда постройка / улучшение здания должна была быть завершена</returns>
        public DateTime GetEndConstructionTime(ProfileBuildingsData data)
        {
            if (IsMaxLevel(data))
                return DateTime.UtcNow;
            
            var currentLevel = GetCurrentLevel(data);
            var ticks = GetStartConstructionTime(data);
            var startDt = new DateTime(ticks);
            var secondsForConstruction = buildingData.levels[currentLevel].constructionTime;
            var endDt = startDt.AddSeconds(secondsForConstruction);
            return endDt;
        }

        /// <returns>Можно ли сейчас завершить постройку / улучшение здания</returns>
        public bool IsConstructionCanBeFinished(ProfileBuildingsData data)
        {
            return DateTime.UtcNow > GetEndConstructionTime(data);
        }

        /// <summary>
        /// Повысить здание до следующего уровня
        /// </summary>
        public void LevelUp(ProfileBuildingsData data)
        {
            if (IsMaxLevel(data))
                return;

            var currentLevel = GetCurrentLevel(data);
            currentLevel++;
            SetCurrentLevel(data, currentLevel);
            SetStartConstructionTime(data, 0);
        }

    }
}
