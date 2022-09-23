using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public abstract class TrainingBuildingBase : BuildingBase
    {
        public abstract IEnumerable<sbyte> GetAllUnits(ProfileBuildingsData data);

        public abstract string GetUnitName(GameSession session, ProfileBuildingsData data, sbyte unitIndex);
        public abstract string GetUnitIcon(ProfileBuildingsData data, sbyte unitIndex);
        public abstract byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex);

        public bool HasFirstTrainingUnit(ProfileBuildingsData data) => GetFirstTrainingUnitIndex(data) != -1;
        public abstract sbyte GetFirstTrainingUnitIndex(ProfileBuildingsData data);
        public abstract void SetFirstTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex);
        public abstract long GetFirstTrainingUnitStartTime(ProfileBuildingsData data);
        public abstract void SetFirstTrainingUnitStartTime(ProfileBuildingsData data, long ticks);

        public bool HasSecondTrainingUnit(ProfileBuildingsData data) => GetSecondTrainingUnitIndex(data) != -1;
        public abstract sbyte GetSecondTrainingUnitIndex(ProfileBuildingsData data);
        public abstract void SetSecondTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex);
        public abstract long GetSecondTrainingUnitStartTime(ProfileBuildingsData data);
        public abstract void SetSecondTrainingUnitStartTime(ProfileBuildingsData data, long ticks);

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            //TODO
            return Localization.Get(session, $"building_{buildingType}_description");
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            //TODO
            return Localization.Get(session, $"building_{buildingType}_description");
        }

    }

}
