using System.Text;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings.General
{
    public class HospitalBuilding : BuildingBase
    {
        public override BuildingType buildingType => BuildingType.Hospital;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.hospitalLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.hospitalLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.hospitalStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.hospitalStartConstructionTime = startConstructionTime;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));

            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return sb.ToString();

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_hospital_health_per_second_header"));
            var levelInfo = (HospitalLevelInfo)buildingData.levels[currentLevel - 1];
            var healthAmount = levelInfo.restoreHealthPerMinute;
            sb.Append($"{Emojis.stats[Stat.Health]} {healthAmount}");

            return sb.ToString();
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));

            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return sb.ToString();

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_hospital_health_per_second_header"));
            var currentLevelInfo = (HospitalLevelInfo)buildingData.levels[currentLevel - 1];
            var currentHealthAmount = currentLevelInfo.restoreHealthPerMinute;
            var nextLevelInfo = (HospitalLevelInfo)buildingData.levels[currentLevel];
            var nextHealthAmount = nextLevelInfo.restoreHealthPerMinute;
            var delta = nextHealthAmount - currentHealthAmount;
            sb.Append($"{Emojis.stats[Stat.Health]} {nextHealthAmount} (<i>+{delta}</i>)");

            return sb.ToString();
        }

        public int GetHealthRestorePerMinute(ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return 0;

            var levelInfo = (HospitalLevelInfo)buildingData.levels[currentLevel - 1];
            return levelInfo.restoreHealthPerMinute;
        }

        public void SetLastRegenTimeAsNow(ProfileBuildingsData data)
        {
            data.hospitalLastHealthRestoreTime = System.DateTime.UtcNow.Ticks;
        }

        public long GetLastRegenTime(ProfileBuildingsData data)
        {
            return data.hospitalLastHealthRestoreTime;
        }

    }
}
