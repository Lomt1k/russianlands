using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Buildings.Craft
{
    public class JewerlyBuilding : CraftBuildingBase
    {
        public override List<ItemType> craftCategories => new List<ItemType>
        {
            ItemType.Ring,
            ItemType.Amulet
        };

        public override BuildingId buildingId => BuildingId.Jewerly;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.jewerlyLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.jewerlyLevel = level;
        }

        protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.jewerlyStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
        {
            data.jewerlyStartConstructionTime = startConstructionTime;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.Append(Localization.Get(session, "building_Jewerly_description"));

            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return sb.ToString();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_craft_items_level_header"));
            var itemLevels = GetCurrentCraftLevels(data);
            sb.Append(itemLevels);

            return sb.ToString();
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.Append(Localization.Get(session, "building_Jewerly_description"));

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_craft_items_level_header"));
            var itemLevels = GetNextCraftLevels(data);
            sb.Append(itemLevels);

            return sb.ToString();
        }

        public override DateTime GetStartCraftTime(ProfileBuildingsData data)
        {
            return data.jewerlyStartCraftTime;
        }

        protected override void SetStartCraftTime(ProfileBuildingsData data, DateTime startCraftTime)
        {
            data.jewerlyStartCraftTime = startCraftTime;
        }

        public override ItemType GetCurrentCraftItemType(ProfileBuildingsData data)
        {
            return data.jewerlyCraftItemType;
        }

        protected override void SetCurrentCraftItemType(ProfileBuildingsData data, ItemType itemType)
        {
            data.jewerlyCraftItemType = itemType;
        }

        public override Rarity GetCurrentCraftItemRarity(ProfileBuildingsData data)
        {
            return data.jewerlyCraftItemRarity;
        }

        protected override void SetCurrentCraftItemRarity(ProfileBuildingsData data, Rarity rarity)
        {
            data.jewerlyCraftItemRarity = rarity;
        }

    }
}
