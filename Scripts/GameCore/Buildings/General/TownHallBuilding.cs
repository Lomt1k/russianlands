using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

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
    }
}
