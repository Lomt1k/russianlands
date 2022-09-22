using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public abstract class TrainingBuildingBase : BuildingBase
    {
        public abstract sbyte unitsCount { get; }

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


        public abstract string GetUnitName(ProfileBuildingsData data, sbyte unitIndex);
        public abstract string GetUnitPrefix(ProfileBuildingsData data, sbyte unitIndex);
        public abstract byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex);
    }

}
