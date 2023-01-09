using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Buildings.Craft
{
    public class WeaponsWorkshopBuilding : CraftBuildingBase
    {
        public override List<ItemType> craftCategories => new List<ItemType>
        {
            ItemType.Sword,
            ItemType.Bow,
            ItemType.Stick
        };

        public override BuildingType buildingType => BuildingType.WeaponsWorkshop;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.weaponsWorkshopLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.weaponsWorkshopLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.weaponsWorkshopStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.weaponsWorkshopStartConstructionTime = startConstructionTime;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            //TODO
            return Localization.Get(session, "building_WeaponsWorkshop_description");
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            //TODO
            return Localization.Get(session, "building_WeaponsWorkshop_description");
        }

        protected override long GetStartCraftTime(ProfileBuildingsData data)
        {
            return data.weaponsWorkshopStartCraftTime;
        }

        protected override void SetStartCraftTime(ProfileBuildingsData data, long startCraftTime)
        {
            data.weaponsWorkshopStartCraftTime = startCraftTime;
        }

        protected override ItemType GetCurrentCraftItemType(ProfileBuildingsData data)
        {
            return (ItemType)data.weaponsWorkshopCraftItemType;
        }

        protected override void SetCurrentCraftItemType(ProfileBuildingsData data, ItemType itemType)
        {
            data.weaponsWorkshopCraftItemType = (sbyte)itemType;
        }

        protected override Rarity GetCurrentCraftItemRarity(ProfileBuildingsData data)
        {
            return (Rarity)data.weaponsWorkshopCraftItemRarity;
        }

        protected override void SetCurrentCraftItemRarity(ProfileBuildingsData data, Rarity rarity)
        {
            data.weaponsWorkshopCraftItemRarity = (byte)rarity;
        }
        
    }
}
