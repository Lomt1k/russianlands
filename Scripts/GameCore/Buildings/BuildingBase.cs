using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public abstract class BuildingBase
    {
        public abstract ConstructionInfo constructionInfo { get; }

        public abstract int GetCurrentLevel(ProfileBuildingsData data);
        public abstract bool IsUnderConstruction(ProfileBuildingsData data);
        public abstract bool IsMaxLevel(ProfileBuildingsData data);
    }
}
