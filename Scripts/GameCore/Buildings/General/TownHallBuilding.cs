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

        public override bool IsMaxLevel(ProfileBuildingsData data)
        {
            return data.townHallLevel == buildingData.levels.Count;
        }

        public override bool IsUnderConstruction(ProfileBuildingsData data)
        {
            return data.townHallStartConstructionTime > 0;
        }
    }
}
