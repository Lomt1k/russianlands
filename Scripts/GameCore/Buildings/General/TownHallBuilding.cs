using System;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings.General
{
    public class TownHallBuilding : BuildingBase
    {
        public override BuildingData constructionInfo => throw new NotImplementedException();

        public override int GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.townHallLevel;
        }

        public override bool IsMaxLevel(ProfileBuildingsData data)
        {
            return data.townHallLevel == constructionInfo.levels.Count;
        }

        public override bool IsUnderConstruction(ProfileBuildingsData data)
        {
            return data.townHallStartConstructionTime > 0;
        }
    }
}
