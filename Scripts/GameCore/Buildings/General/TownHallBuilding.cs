using System;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings.General
{
    public class TownHallBuilding : BuildingBase
    {
        public override BuildingData buildingData => GameDataBase.GameDataBase.instance.buildings[(int)BuildingType.TownHall];

        public override int GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.townHallLevel;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.townHallStartConstructionTime;
        }
    }
}
