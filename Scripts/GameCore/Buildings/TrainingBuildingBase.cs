using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Dialogs.Town.Buildings.TrainingBuildingDialog;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Buildings;

public abstract class TrainingBuildingBase : BuildingBase
{
    public abstract int GetRequiredTrainingTime(byte currentLevel);
    public abstract IEnumerable<sbyte> GetAllUnits(ProfileBuildingsData data);

    public abstract string GetUnitName(GameSession session, ProfileBuildingsData data, sbyte unitIndex);
    public abstract string GetUnitIcon(ProfileBuildingsData data, sbyte unitIndex);
    public abstract byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex);
    public abstract string GetInfoAboutUnitTraining(GameSession session, ProfileBuildingsData data, sbyte unitIndex);

    public bool HasFirstTrainingUnit(ProfileBuildingsData data)
    {
        return GetFirstTrainingUnitIndex(data) != -1;
    }

    public abstract sbyte GetFirstTrainingUnitIndex(ProfileBuildingsData data);
    public abstract void SetFirstTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex);
    public abstract DateTime GetFirstTrainingUnitStartTime(ProfileBuildingsData data);
    public abstract void SetFirstTrainingUnitStartTime(ProfileBuildingsData data, DateTime dateTime);
    public abstract void LevelUpFirst(GameSession session, ProfileBuildingsData data);

    public bool HasSecondTrainingUnit(ProfileBuildingsData data)
    {
        return GetSecondTrainingUnitIndex(data) != -1;
    }

    public abstract sbyte GetSecondTrainingUnitIndex(ProfileBuildingsData data);
    public abstract void SetSecondTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex);
    public abstract DateTime GetSecondTrainingUnitStartTime(ProfileBuildingsData data);
    public abstract void SetSecondTrainingUnitStartTime(ProfileBuildingsData data, DateTime dateTime);
    public abstract void LevelUpSecond(GameSession session, ProfileBuildingsData data);

    /// <returns>Дата, когда тренировка первого юнита должна была быть завершена</returns>
    public DateTime GetFirstTrainingUnitEndTime(ProfileBuildingsData data)
    {
        var unitIndex = GetFirstTrainingUnitIndex(data);
        var currentLevel = GetUnitLevel(data, unitIndex);
        var secondsForTraining = GetRequiredTrainingTime(currentLevel);
        var startDt = GetFirstTrainingUnitStartTime(data);
        var endDt = startDt.AddSeconds(secondsForTraining);
        return endDt;
    }

    /// <returns>Дата, когда тренировка второго юнита должна была быть завершена</returns>
    public DateTime GetSecondTrainingUnitEndTime(ProfileBuildingsData data)
    {
        var unitIndex = GetSecondTrainingUnitIndex(data);
        var currentLevel = GetUnitLevel(data, unitIndex);
        var secondsForTraining = GetRequiredTrainingTime(currentLevel);
        var startDt = GetSecondTrainingUnitStartTime(data);
        var endDt = startDt.AddSeconds(secondsForTraining);
        return endDt;
    }

    public override Dictionary<string, Func<Task>> GetSpecialButtons(GameSession session, ProfileBuildingsData data)
    {
        var result = new Dictionary<string, Func<Task>>();

        if (!IsUnderConstruction(data))
        {
            result.Add(Emojis.ElementTraining + Localization.Get(session, "building_training_open_dialog_button"),
                () => new TrainingBuildingDialog(session, this, data).Start());
        }

        return result;
    }

    public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));
        sb.AppendLine();

        var maxUnitLevel = GetCurrentMaxUnitLevel(data);
        var formatted = Localization.Get(session, "building_training_level_limit", maxUnitLevel);
        sb.Append(Emojis.ElementTraining + formatted);
        return sb.ToString();
    }

    public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));
        sb.AppendLine();

        var currentValue = GetCurrentMaxUnitLevel(data);
        var nextValue = GetNextMaxUnitLevel(data);
        var delta = nextValue - currentValue;
        var hideDelta = !IsBuilt(data);
        var dynamicData = nextValue + (hideDelta ? string.Empty : $" (<i>+{delta}</i>)");
        var formatted = Localization.Get(session, "building_training_level_limit", dynamicData);
        sb.Append(Emojis.ElementTraining + formatted);
        return sb.ToString();
    }

    public int GetCurrentMaxUnitLevel(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return 0;

        var levelInfo = (TrainingLevelInfo)buildingData.levels[currentLevel - 1];
        return levelInfo.maxUnitLevel;
    }

    public int GetNextMaxUnitLevel(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        var levelInfo = (TrainingLevelInfo)buildingData.levels[currentLevel];
        return levelInfo.maxUnitLevel;
    }

    public bool HasFreeTrainingSlot(ProfileBuildingsData data)
    {
        return !HasFirstTrainingUnit(data) || !HasSecondTrainingUnit(data);
    }

    public override bool HasImportantUpdatesInternal(ProfileBuildingsData data)
    {
        return IsTrainingCanBeFinished(data, out _, out _);
    }

    protected override List<string> GetUpdatesInternal(GameSession session, ProfileBuildingsData data, bool onlyImportant)
    {
        var updates = new List<string>();
        IsTrainingCanBeFinished(data, out var isFirstTrainingCanBeFinished, out var isSecondTrainingCanBeFinished);

        if (HasFirstTrainingUnit(data))
        {
            var unitIndex = GetFirstTrainingUnitIndex(data);
            var unitName = GetUnitName(session, data, unitIndex);
            if (isFirstTrainingCanBeFinished)
            {
                LevelUpFirst(session, data);
                var currentLevel = GetUnitLevel(data, unitIndex);
                var update = Localization.Get(session, "building_training_end", unitName, currentLevel);
                updates.Add(update);
            }
            else if (!onlyImportant)
            {
                var endTime = GetFirstTrainingUnitEndTime(data);
                var timeToEnd = endTime - DateTime.UtcNow;
                var update = Localization.Get(session, $"building_training_progress", unitName, timeToEnd.GetView(session));
                updates.Add(update);
            }
        }

        if (HasSecondTrainingUnit(data))
        {
            var unitIndex = GetSecondTrainingUnitIndex(data);
            var unitName = GetUnitName(session, data, unitIndex);
            if (isSecondTrainingCanBeFinished)
            {
                LevelUpSecond(session, data);
                var currentLevel = GetUnitLevel(data, unitIndex);
                var update = Localization.Get(session, "building_training_end", unitName, currentLevel);
                updates.Add(update);
            }
            else if (!onlyImportant)
            {
                var endTime = GetSecondTrainingUnitEndTime(data);
                var timeToEnd = endTime - DateTime.UtcNow;
                var update = Localization.Get(session, $"building_training_progress", unitName, timeToEnd.GetView(session));
                updates.Add(update);
            }
        }

        return updates;
    }

    public bool IsTrainingCanBeFinished(ProfileBuildingsData data, out bool firstTraining, out bool secondTraining)
    {
        firstTraining = false;
        secondTraining = false;

        if (HasFirstTrainingUnit(data))
        {
            var unitIndex = GetFirstTrainingUnitIndex(data);
            var unitLevel = GetUnitLevel(data, unitIndex);
            var requiredSeconds = GetRequiredTrainingTime(unitLevel);
            var startDt = GetFirstTrainingUnitStartTime(data);
            var trainingSeconds = (DateTime.UtcNow - startDt).TotalSeconds;
            firstTraining = trainingSeconds > requiredSeconds;
        }
        if (HasSecondTrainingUnit(data))
        {
            var unitIndex = GetSecondTrainingUnitIndex(data);
            var unitLevel = GetUnitLevel(data, unitIndex);
            var requiredSeconds = GetRequiredTrainingTime(unitLevel);
            var startDt = GetSecondTrainingUnitStartTime(data);
            var trainingSeconds = (DateTime.UtcNow - startDt).TotalSeconds;
            secondTraining = trainingSeconds > requiredSeconds;
        }

        return firstTraining || secondTraining;
    }

    protected override void OnConstructionStart(ProfileBuildingsData data)
    {
        SetFirstTrainingUnitIndex(data, -1);
        SetFirstTrainingUnitStartTime(data, DateTime.MinValue);
        SetSecondTrainingUnitIndex(data, -1);
        SetSecondTrainingUnitStartTime(data, DateTime.MinValue);
    }

    public override string? GetSpecialConstructionWarning(ProfileBuildingsData data, GameSession session)
    {
        var hasActiveTraining = GetFirstTrainingUnitIndex(data) != -1 || GetSecondTrainingUnitIndex(data) != -1;
        return hasActiveTraining ? Localization.Get(session, "building_training_break_warning") : null;
    }

}
