using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public class PlayerBuildings
    {
        private GameSession _session;
        private ProfileBuildingsData _buildingsData;

        public PlayerBuildings(GameSession session)
        {
            _session = session;
            _buildingsData = session.profile.buildingsData;
        }

        // TODO: когда будут готовы все здания - заменить на пробег по enum BuildingType 
        public IEnumerable<BuildingBase> GetAllBuildings()
        {
            yield return BuildingType.TownHall.GetBuilding();
        }

        public IEnumerable<BuildingBase> GetBuildingsByCategory(BuildingCategory category)
        {
            switch (category)
            {
                case BuildingCategory.General:
                    yield return BuildingType.TownHall.GetBuilding();
                    break;
            }
        }


    }
}
