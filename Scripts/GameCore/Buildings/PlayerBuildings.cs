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

        public IEnumerable<BuildingBase> GetGeneralBuildings()
        {
            yield return BuildingType.TownHall.GetBuilding();
        }


    }
}
