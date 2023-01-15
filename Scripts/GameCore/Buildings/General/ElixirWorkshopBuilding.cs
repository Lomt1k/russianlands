using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings.General
{
    public class ElixirWorkshopBuilding : BuildingBase
    {
        public override BuildingType buildingType => BuildingType.ElixirWorkshop;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.elixirWorkshopLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.elixirWorkshopLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.elixirWorkshopStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.elixirWorkshopStartConstructionTime = startConstructionTime;
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            //TODO: not implemented
            return "TODO";
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            //TODO: not implemented
            return "TODO";
        }

    }
}
