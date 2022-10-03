using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings.Training
{
    public class WarriorTrainingBuilding : TrainingBuildingBase
    {
        public override BuildingType buildingType => BuildingType.WarriorTraining;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.warriorTrainingLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.warriorTrainingLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.warriorTrainingStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.warriorTrainingStartConstructionTime = startConstructionTime;
        }

        public override IEnumerable<sbyte> GetAllUnits(ProfileBuildingsData data)
        {
            yield return 0;
        }

        public override string GetUnitName(GameSession session, ProfileBuildingsData data, sbyte unitIndex)
        {
            return session.player.nickname;
        }

        public override string GetUnitIcon(ProfileBuildingsData data, sbyte unitIndex)
        {
            return Emojis.characters[CharIcon.Male];
        }

        public override byte GetUnitLevel(ProfileBuildingsData data, sbyte unitIndex)
        {
            return data.session.player.level;
        }

        public override sbyte GetFirstTrainingUnitIndex(ProfileBuildingsData data)
        {
            return data.warriorTrainingFirstUnitIndex;
        }

        public override void SetFirstTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
        {
            data.warriorTrainingFirstUnitIndex = unitIndex;
        }

        public override long GetFirstTrainingUnitStartTime(ProfileBuildingsData data)
        {
            return data.warriorTrainingFirstUnitStartTime;
        }

        public override void SetFirstTrainingUnitStartTime(ProfileBuildingsData data, long ticks)
        {
            data.warriorTrainingFirstUnitStartTime = ticks;
        }

        public override sbyte GetSecondTrainingUnitIndex(ProfileBuildingsData data)
        {
            return -1; //not used
        }

        public override void SetSecondTrainingUnitIndex(ProfileBuildingsData data, sbyte unitIndex)
        {
            //not used
        }

        public override long GetSecondTrainingUnitStartTime(ProfileBuildingsData data)
        {
            return 0; //not used
        }

        public override void SetSecondTrainingUnitStartTime(ProfileBuildingsData data, long ticks)
        {
            //not used
        }

        public override void LevelUpFirst(GameSession session, ProfileBuildingsData data)
        {
            session.profile.data.level += 1;
            var playerStats = (PlayerStats)session.player.unitStats;
            playerStats.Recalculate();
            playerStats.SetFullHealth();

            SetFirstTrainingUnitIndex(data, -1);
            SetFirstTrainingUnitStartTime(data, 0);
        }

        public override void LevelUpSecond(GameSession session, ProfileBuildingsData data)
        {
            //not used
        }

        public override int GetRequiredTrainingTime(byte currentLevel)
        {
            return ResourceHelper.GetWarriorTrainingTimeInSeconds(currentLevel);
        }

        public override string GetInfoAboutUnitTraining(GameSession session, ProfileBuildingsData data, sbyte unitIndex)
        {
            var sb = new StringBuilder();
            var currentHealth = session.player.unitStats.maxHP;
            var currentUnitLevel = GetUnitLevel(data, unitIndex);
            var maxUnitLevel = GetCurrentMaxUnitLevel(data);
            if (currentUnitLevel >= maxUnitLevel)
            {
                sb.AppendLine(Localization.Get(session, "building_training_max_health_header"));
                sb.Append(Emojis.stats[Stat.Health] + $" {currentHealth.View()}");
                return sb.ToString();
            }

            var bonusPerLevel = PlayerStats.HEALTH_PER_LEVEL;
            var nextHealth = currentHealth + bonusPerLevel;

            sb.AppendLine(Localization.Get(session, "building_training_max_health_header"));
            sb.Append(Emojis.stats[Stat.Health] + $" {nextHealth.View()} (<i>+{bonusPerLevel.View()}</i>)");
            return sb.ToString();
        }

    }
}
