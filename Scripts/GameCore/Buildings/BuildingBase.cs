using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Buildings;

public abstract class BuildingBase
{
    private static readonly GameDataHolder gameDataBase = Services.ServiceLocator.Get<GameDataHolder>();

    public abstract BuildingId buildingId { get; }
    public BuildingData buildingData => gameDataBase.buildings[buildingId];

    public virtual Dictionary<string, Func<Task>> GetSpecialButtons(GameSession session, ProfileBuildingsData data)
    {
        return new Dictionary<string, Func<Task>>();
    }

    public abstract byte GetCurrentLevel(ProfileBuildingsData data);
    public abstract string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data);
    public abstract string GetNextLevelInfo(GameSession session, ProfileBuildingsData data);
    protected abstract void SetCurrentLevel(ProfileBuildingsData data, byte level);
    protected abstract DateTime GetStartConstructionTime(ProfileBuildingsData data);
    protected abstract void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime);


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

    public List<string> GetUpdates(GameSession session, ProfileBuildingsData data, bool onlyImportant)
    {
        var updates = new List<string>();

        if (IsUnderConstruction(data))
        {
            if (IsConstructionCanBeFinished(data))
            {
                LevelUp(data);
                var currentLevel = GetCurrentLevel(data);
                var update = currentLevel > 1
                    ? Localization.Get(session, "building_construction_end", currentLevel)
                    : Localization.Get(session, "building_build_end");
                updates.Add(update);
            }
            else
            {
                var endTime = GetEndConstructionTime(data);
                var timeToEnd = endTime - DateTime.UtcNow;
                var key = IsBuilt(data) ? "construction" : "build";
                var update = Localization.Get(session, $"building_{key}_progress", timeToEnd.GetView(session));
                updates.Add(update);
            }
        }

        if (!IsBuilt(data))
            return updates;

        var internalUpdates = GetUpdatesInternal(session, data, onlyImportant);
        updates.AddRange(internalUpdates);
        return updates;
    }

    protected virtual List<string> GetUpdatesInternal(GameSession session, ProfileBuildingsData data, bool onlyImportant)
    {
        return new List<string>();
    }

    public string GetLocalizedName(GameSession session, ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        return Localization.Get(session, "building_name_" + buildingId.ToString())
            + (currentLevel > 0 ? Localization.Get(session, "level_suffix", currentLevel) : string.Empty);
    }

    public string GetNextLevelLocalizedName(GameSession session, ProfileBuildingsData data)
    {
        if (IsMaxLevel(data))
            return string.Empty;

        var nextLevel = GetCurrentLevel(data) + 1;
        return Localization.Get(session, "building_name_" + buildingId.ToString())
            + (nextLevel > 0 ? Localization.Get(session, "level_suffix", nextLevel) : string.Empty);
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
        return GetStartConstructionTime(data) > DateTime.MinValue;
    }

    /// <summary>
    /// Запускает процесс постройки / улучшения здания
    /// </summary>
    public void StartConstruction(ProfileBuildingsData data)
    {
        if (IsNextLevelUnlocked(data))
        {
            SetStartConstructionTime(data, DateTime.UtcNow);
            OnConstructionStart(data);
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

    // Тут можно определить дополнительную логику, которая будет вызываться при начале строительства (улучшения) здания
    protected virtual void OnConstructionStart(ProfileBuildingsData data) { }

    /// <returns>Дата, когда постройка / улучшение здания должна была быть завершена</returns>
    public DateTime GetEndConstructionTime(ProfileBuildingsData data)
    {
        if (IsMaxLevel(data))
            return DateTime.UtcNow;

        var currentLevel = GetCurrentLevel(data);
        var startDt = GetStartConstructionTime(data);
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

        var startConstructionTime = GetStartConstructionTime(data);
        var endConstructionTime = GetEndConstructionTime(data);
        OnConstructionEnd(data, startConstructionTime, endConstructionTime);

        var currentLevel = GetCurrentLevel(data);
        currentLevel++;
        SetCurrentLevel(data, currentLevel);
        SetStartConstructionTime(data, DateTime.MinValue);
    }

    // Тут можно определить дополнительную логику, которая будет вызываться при завершении строительства (улучшения)
    protected virtual void OnConstructionEnd(ProfileBuildingsData data, DateTime startConstructionTime, DateTime endConstructionTime) { }

    /// <returns>Заблокирована ли сейчас возможность начать улучшение здания</returns>
    public virtual bool IsStartConstructionBlocked(ProfileBuildingsData data, out string blockReasonMessage)
    {
        blockReasonMessage = string.Empty;
        return false;
    }

    // Этот метод используется только для чита!
    public void Cheat_SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        SetCurrentLevel(data, level);
        SetStartConstructionTime(data, DateTime.MinValue);
    }

}
