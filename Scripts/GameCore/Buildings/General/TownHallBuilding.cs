using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings.General
{
    public class TownHallBuilding : BuildingBase
    {
        public override BuildingType buildingType => BuildingType.TownHall;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.townHallLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.townHallLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.townHallStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.townHallStartConstructionTime = startConstructionTime;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            return Localization.Get(session, "building_TownHall_description");
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            // TODO: Добавить инфу о следующем уровне
            return Localization.Get(session, "building_TownHall_description");
        }
    }
}
