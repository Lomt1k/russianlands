using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings.CraftBuildingDialog;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public abstract class CraftBuildingBase : BuildingBase
    {
        public abstract List<ItemType> craftCategories { get; }

        protected abstract long GetStartCraftTime(ProfileBuildingsData data);
        protected abstract void SetStartCraftTime(ProfileBuildingsData data, long startCraftTime);
        protected abstract ItemType GetCurrentCraftItemType(ProfileBuildingsData data);
        protected abstract void SetCurrentCraftItemType(ProfileBuildingsData data, ItemType itemType);
        protected abstract Rarity GetCurrentCraftItemRarity(ProfileBuildingsData data);
        protected abstract void SetCurrentCraftItemRarity(ProfileBuildingsData data, Rarity rarity);

        /// <returns>Ведётся ли сейчас изготовление предмета</returns>
        public bool IsCraftStarted(ProfileBuildingsData data)
        {
            return GetStartCraftTime(data) > 0;
        }

        /// <returns>Время изготовления предмета в зависимости от его редкости</returns>
        public int GetCraftTimeInSeconds(ProfileBuildingsData data, Rarity rarity)
        {
            var currentLevel = GetCurrentLevel(data);
            var levelInfo = (CraftLevelInfo)buildingData.levels[currentLevel - 1];
            return rarity switch
            {
                Rarity.Rare => levelInfo.rareCraft_Time,
                Rarity.Epic => levelInfo.epicCraft_Time,
                Rarity.Legendary => levelInfo.legendaryCraft_Time,
                _ => 0
            };
        }

        /// <returns>Дата, когда изготовление предмета должно быть заверено</returns>
        public DateTime GetEndCraftTime(ProfileBuildingsData data)
        {
            var ticks = GetStartCraftTime(data);
            var startDt = new DateTime(ticks);
            var rarity = GetCurrentCraftItemRarity(data);
            var secondsForCraft = GetCraftTimeInSeconds(data, rarity);
            var endDt = startDt.AddSeconds(secondsForCraft);
            return endDt;
        }

        /// <returns>Можно ли сейчас завершить изготовление здания</returns>
        public bool IsCraftCanBeFinished(ProfileBuildingsData data)
        {
            return DateTime.UtcNow > GetEndCraftTime(data);
        }

        public override bool HasImportantUpdatesInternal(ProfileBuildingsData data)
        {
            return IsCraftStarted(data) && IsCraftCanBeFinished(data);
        }

        protected override List<string> GetUpdatesInternal(GameSession session, ProfileBuildingsData data, bool onlyImportant)
        {
            var updates = new List<string>();

            // TODO

            return updates;
        }

        public override Dictionary<string, Func<Task>> GetSpecialButtons(GameSession session, ProfileBuildingsData data)
        {
            var result = new Dictionary<string, Func<Task>>();

            if (!IsUnderConstruction(data))
            {
                result.Add($"{Emojis.menuItems[MenuItem.Craft]} {Localization.Get(session, "menu_item_craft")}",
                    () => new CraftBuildingDialog(session, this).Start());
            }

            return result;
        }

    }
}
