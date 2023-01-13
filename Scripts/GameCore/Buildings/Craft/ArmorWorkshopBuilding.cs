﻿using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.Generators;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Buildings.Craft
{
    public class ArmorWorkshopBuilding : CraftBuildingBase
    {
        public override List<ItemType> craftCategories => new List<ItemType>
        {
            ItemType.Armor,
            ItemType.Helmet,
            ItemType.Boots,
            ItemType.Shield
        };

        public override BuildingType buildingType => BuildingType.ArmorWorkshop;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.armorWorkshopLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.armorWorkshopLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.armorWorkshopStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.armorWorkshopStartConstructionTime = startConstructionTime;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.Append(Localization.Get(session, "building_ArmorWorkshop_description"));

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
            sb.Append(Localization.Get(session, "building_ArmorWorkshop_description"));

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_craft_items_level_header"));
            var itemLevels = GetNextCraftLevels(data);
            sb.Append(itemLevels);

            return sb.ToString();
        }

        public override long GetStartCraftTime(ProfileBuildingsData data)
        {
            return data.armorWorkshopStartCraftTime;
        }

        protected override void SetStartCraftTime(ProfileBuildingsData data, long startCraftTime)
        {
            data.armorWorkshopStartCraftTime = startCraftTime;
        }

        public override ItemType GetCurrentCraftItemType(ProfileBuildingsData data)
        {
            return (ItemType)data.armorWorkshopCraftItemType;
        }

        protected override void SetCurrentCraftItemType(ProfileBuildingsData data, ItemType itemType)
        {
            data.armorWorkshopCraftItemType = (sbyte)itemType;
        }

        public override Rarity GetCurrentCraftItemRarity(ProfileBuildingsData data)
        {
            return (Rarity)data.armorWorkshopCraftItemRarity;
        }

        protected override void SetCurrentCraftItemRarity(ProfileBuildingsData data, Rarity rarity)
        {
            data.armorWorkshopCraftItemRarity = (byte)rarity;
        }

    }
}
