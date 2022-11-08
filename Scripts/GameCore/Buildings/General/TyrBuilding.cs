﻿using System.Text;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings.General
{
    public class TyrBuilding : BuildingBase
    {
        public override BuildingType buildingType => BuildingType.Tyr;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.tyrLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.tyrLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.tyrConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.tyrConstructionTime = startConstructionTime;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, $"building_{buildingType}_description"));

            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return sb.ToString();

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_tyr_arrows_header"));
            var levelInfo = (TyrLevelInfo)buildingData.levels[currentLevel - 1];
            var arrowsAmount = levelInfo.arrowsAmount;
            sb.Append($"{Emojis.stats[Stat.Arrows]} {arrowsAmount}");

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
            sb.AppendLine(Localization.Get(session, "building_tyr_arrows_header"));
            var currentLevelInfo = (TyrLevelInfo)buildingData.levels[currentLevel - 1];
            var currentArrowsAmount = currentLevelInfo.arrowsAmount;
            var nextLevelInfo = (TyrLevelInfo)buildingData.levels[currentLevel];
            var nextArrowsAmount = nextLevelInfo.arrowsAmount;
            var delta = nextArrowsAmount - currentArrowsAmount;
            sb.Append($"{Emojis.stats[Stat.Arrows]} {nextArrowsAmount} (<i>+{delta}</i>)");

            return sb.ToString();
        }

        public int GetArrowsAmount(GameSession session, ProfileBuildingsData data)
        {
            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return 0;

            var currentLevelInfo = (TyrLevelInfo)buildingData.levels[currentLevel - 1];
            return currentLevelInfo.arrowsAmount;
        }

    }
}
