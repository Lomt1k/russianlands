using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Buildings.Craft
{
    public class ScribesHouseBuilding : CraftBuildingBase
    {
        public override List<ItemType> craftCategories => new List<ItemType>
        {
            ItemType.Scroll
        };

        public override BuildingType buildingType => BuildingType.ScribesHouse;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.scribesHouseLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.scribesHouseLevel = level;
        }

        protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.scribesHouseStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
        {
            data.scribesHouseStartConstructionTime = startConstructionTime;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.Append(Localization.Get(session, "building_ScribesHouse_description"));

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
            sb.Append(Localization.Get(session, "building_ScribesHouse_description"));

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_craft_items_level_header"));
            var itemLevels = GetNextCraftLevels(data);
            sb.Append(itemLevels);

            return sb.ToString();
        }

        public override DateTime GetStartCraftTime(ProfileBuildingsData data)
        {
            return data.scribesHouseStartCraftTime;
        }

        protected override void SetStartCraftTime(ProfileBuildingsData data, DateTime startCraftTime)
        {
            data.scribesHouseStartCraftTime = startCraftTime;
        }

        public override ItemType GetCurrentCraftItemType(ProfileBuildingsData data)
        {
            return ItemType.Scroll;
        }

        protected override void SetCurrentCraftItemType(ProfileBuildingsData data, ItemType itemType)
        {
            //ingrored
        }

        public override Rarity GetCurrentCraftItemRarity(ProfileBuildingsData data)
        {
            return (Rarity)data.scribesHouseCraftItemRarity;
        }

        protected override void SetCurrentCraftItemRarity(ProfileBuildingsData data, Rarity rarity)
        {
            data.scribesHouseCraftItemRarity = (byte)rarity;
        }

    }
}
