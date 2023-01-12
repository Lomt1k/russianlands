using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.Generators;
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

        public override BuildingType buildingType => BuildingType.Jewerly;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.jewerlyLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.jewerlyLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.jewerlyStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
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

        public override long GetStartCraftTime(ProfileBuildingsData data)
        {
            return data.jewerlyStartCraftTime;
        }

        protected override void SetStartCraftTime(ProfileBuildingsData data, long startCraftTime)
        {
            data.jewerlyStartCraftTime = startCraftTime;
        }

        public override ItemType GetCurrentCraftItemType(ProfileBuildingsData data)
        {
            return (ItemType)data.jewerlyCraftItemType;
        }

        protected override void SetCurrentCraftItemType(ProfileBuildingsData data, ItemType itemType)
        {
            data.jewerlyCraftItemType = (sbyte)itemType;
        }

        public override Rarity GetCurrentCraftItemRarity(ProfileBuildingsData data)
        {
            return (Rarity)data.jewerlyCraftItemRarity;
        }

        protected override void SetCurrentCraftItemRarity(ProfileBuildingsData data, Rarity rarity)
        {
            data.jewerlyCraftItemRarity = (byte)rarity;
        }

    }
}
