using System;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class HealthRegenerationController
    {
        private Player _player;

        private static HospitalBuilding hospital => (HospitalBuilding)BuildingType.Hospital.GetBuilding();
        private ProfileBuildingsData buildingsData => _player.session.profile.buildingsData;

        public HealthRegenerationController(Player player)
        {
            _player = player;
        }

        public void InvokeRegen()
        {
            if (_player.unitStats.isFullHealth)
                return;

            var prevRegenTime = hospital.GetLastRegenTime(buildingsData);
            var prevRegenTimeDt = new DateTime(prevRegenTime);
            var minutes = (DateTime.UtcNow - prevRegenTimeDt).TotalMinutes;
            var healthPerMinute = hospital.GetHealthRestorePerMinute(buildingsData);
            var heathAmount = (int)Math.Round(minutes * healthPerMinute);
            _player.unitStats.RestoreHealth(heathAmount);
            SetLastRegenTimeAsNow();
        }

        public void SetLastRegenTimeAsNow()
        {
            hospital.SetLastRegenTimeAsNow(buildingsData);
        }

    }
}
