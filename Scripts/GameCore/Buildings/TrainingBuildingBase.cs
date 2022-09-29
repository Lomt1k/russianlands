﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Buildings.TrainingBuildingDialog;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public abstract class TrainingBuildingBase : BuildingBase
    {
        public abstract int GetRequiredTrainingTime(byte currentLevel);
        public abstract IEnumerable<sbyte> GetAllUnits(ProfileBuildingsData data);

        public abstract string GetUnitName(GameSession session, ProfileBuildingsData data, sbyte unitIndex);
        public abstract string GetUnitIcon(ProfileBuildingsData data, sbyte unitIndex);
        public abstract byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex);

        public bool HasFirstTrainingUnit(ProfileBuildingsData data) => GetFirstTrainingUnitIndex(data) != -1;
        public abstract sbyte GetFirstTrainingUnitIndex(ProfileBuildingsData data);
        public abstract void SetFirstTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex);
        public abstract long GetFirstTrainingUnitStartTime(ProfileBuildingsData data);
        public abstract void SetFirstTrainingUnitStartTime(ProfileBuildingsData data, long ticks);
        public abstract void LevelUpFirst(GameSession session, ProfileBuildingsData data);

        public bool HasSecondTrainingUnit(ProfileBuildingsData data) => GetSecondTrainingUnitIndex(data) != -1;
        public abstract sbyte GetSecondTrainingUnitIndex(ProfileBuildingsData data);
        public abstract void SetSecondTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex);
        public abstract long GetSecondTrainingUnitStartTime(ProfileBuildingsData data);
        public abstract void SetSecondTrainingUnitStartTime(ProfileBuildingsData data, long ticks);
        public abstract void LevelUpSecond(GameSession session, ProfileBuildingsData data);

        public override Dictionary<string, Func<Task>> GetSpecialButtons(GameSession session, ProfileBuildingsData data)
        {
            return new Dictionary<string, Func<Task>>
            {
                { Localization.Get(session, "building_training_open_dialog_button"), () => new TrainingBuildingDialog(session, this, data).Start()}
            };
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));
            sb.AppendLine();

            var maxUnitLevel = GetCurrentMaxUnitLevel(data);
            var formatted = string.Format(Localization.Get(session, "building_training_level_limit"), maxUnitLevel);
            sb.Append($"{Emojis.characters[CharIcon.Abstract]} {formatted}");
            return sb.ToString();
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));
            sb.AppendLine();

            var currentValue = GetCurrentMaxUnitLevel(data);
            var nextValue = GetNextMaxUnitLevel(data);
            var delta = nextValue - currentValue;
            bool hideDelta = !IsBuilt(data);
            var dynamicData = nextValue + (hideDelta ? string.Empty : $" (<i>+{delta}</i>)");
            var formatted = string.Format(Localization.Get(session, "building_training_level_limit"), dynamicData);
            sb.Append($"{Emojis.characters[CharIcon.Abstract]} {formatted}");
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
            return IsTrainingCanBeFinished(data, out var firstTraining, out var secondTraining);
        }

        protected override List<string> GetUpdatesInternal(GameSession session, ProfileBuildingsData data)
        {
            var result = new List<string>();
            IsTrainingCanBeFinished(data, out var isFirstTrainingCanBeFinished, out var isSecondTrainingCanBeFinished);

            if (HasFirstTrainingUnit(data))
            {
                var unitIndex = GetFirstTrainingUnitIndex(data);
                if (isFirstTrainingCanBeFinished)
                {
                    LevelUpFirst(session, data);
                    //TODO
                }
                else
                {
                    //TODO
                }
            }

            if (HasSecondTrainingUnit(data))
            {
                if (isSecondTrainingCanBeFinished)
                {
                    LevelUpSecond(session, data);
                    //TODO
                }
                else
                {
                    //TODO
                }
            }

            return result;
        }

        public bool IsTrainingCanBeFinished(ProfileBuildingsData data, out bool firstTraining, out bool secondTraining)
        {
            firstTraining = false;
            secondTraining = false;

            if (HasFirstTrainingUnit(data))
            {
                var unitIndex = GetFirstTrainingUnitIndex(data);
                var unitLevel = GetUnitLevel(data, unitIndex) ;
                var requiredSeconds = GetRequiredTrainingTime(unitLevel);
                var startTicks = GetFirstTrainingUnitStartTime(data);
                var startDt = new DateTime(startTicks);
                var trainingSeconds = (DateTime.UtcNow - startDt).TotalSeconds;
                firstTraining = trainingSeconds > requiredSeconds;
            }
            if (HasSecondTrainingUnit(data))
            {
                var unitIndex = GetSecondTrainingUnitIndex(data);
                var unitLevel = GetUnitLevel(data, unitIndex);
                var requiredSeconds = GetRequiredTrainingTime(unitLevel);
                var startTicks = GetSecondTrainingUnitStartTime(data);
                var startDt = new DateTime(startTicks);
                var trainingSeconds = (DateTime.UtcNow - startDt).TotalSeconds;
                secondTraining = trainingSeconds > requiredSeconds;
            }

            return firstTraining || secondTraining;
        }

    }

}
