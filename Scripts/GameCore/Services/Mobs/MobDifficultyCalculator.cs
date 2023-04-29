using System;
using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.GameCore.Services.Mobs
{
    public static class MobDifficultyCalculator
    {
        private const int MaxDifficultyChangeBySkillLevel = 2;

        public static MobDifficulty GetActualDifficultyForPlayer(Player player)
        {
            var minDifficultyByQuests = GetMinimumDifficultyByQuestProgress(player);
            var minDifficultyByItems = GetMinimumDifficultyByPlayerItems(player);
            var minDifficulty = minDifficultyByQuests > minDifficultyByItems ? minDifficultyByQuests : minDifficultyByItems;

            var difficulty = (int)GetDifficultyByPlayerLevel(player.level);
            if (player.buildings.HasBuilding(BuildingType.ElixirWorkshop) || player.buildings.GetBuildingLevel(BuildingType.TownHall) > 5)
            {
                var bySkill = (int)GetDifficultyByAverageSkillLevel(player);
                if (Math.Abs(bySkill - difficulty) > MaxDifficultyChangeBySkillLevel)
                {
                    bySkill = bySkill > difficulty ? difficulty + 2 : difficulty - 2;
                }
                difficulty = bySkill;
            }

            var result = (MobDifficulty)difficulty;
            return result > minDifficulty ? result : minDifficulty;
        }

        private static MobDifficulty GetMinimumDifficultyByQuestProgress(Player player)
        {
            var playerQuestsProgress = player.session.profile.dynamicData.quests;
            if (playerQuestsProgress.IsCompleted(QuestType.Loc_07)) return MobDifficulty.HALL_8_END;
            if (playerQuestsProgress.IsCompleted(QuestType.Loc_06)) return MobDifficulty.HALL_7_END;
            if (playerQuestsProgress.IsCompleted(QuestType.Loc_05)) return MobDifficulty.HALL_6_END;
            if (playerQuestsProgress.IsCompleted(QuestType.Loc_04)) return MobDifficulty.HALL_5_END;
            if (playerQuestsProgress.IsCompleted(QuestType.Loc_03)) return MobDifficulty.HALL_4_END;
            if (playerQuestsProgress.IsCompleted(QuestType.Loc_02)) return MobDifficulty.HALL_3_END;
            return MobDifficulty.HALL_3_START;
        }

        private static MobDifficulty GetDifficultyByPlayerLevel(byte level)
        {
            return level switch
            {
                >= 34 => MobDifficulty.HALL_8_END,
                >= 32 => MobDifficulty.HALL_8_MID,
                >= 29 => MobDifficulty.HALL_8_START,
                >= 27 => MobDifficulty.HALL_7_END,
                >= 24 => MobDifficulty.HALL_7_MID,
                >= 21 => MobDifficulty.HALL_7_START,
                >= 20 => MobDifficulty.HALL_6_END,
                >= 18 => MobDifficulty.HALL_6_MID,
                >= 16 => MobDifficulty.HALL_6_START,
                >= 14 => MobDifficulty.HALL_5_END,
                >= 11 => MobDifficulty.HALL_5_START,
                >= 10 => MobDifficulty.HALL_4_END,
                >= 8 => MobDifficulty.HALL_4_START,
                >= 6 => MobDifficulty.HALL_3_END,
                _ => MobDifficulty.HALL_3_START
            };
        }

        private static MobDifficulty GetDifficultyByAverageSkillLevel(Player player)
        {
            var skills = player.skills.GetAverageSkillLevel();
            return skills switch
            {
                >= 95 => MobDifficulty.END_GAME_PLUS,
                >= 85 => MobDifficulty.END_GAME,
                >= 70 => MobDifficulty.HALL_8_END,
                >= 65 => MobDifficulty.HALL_8_MID,
                >= 60 => MobDifficulty.HALL_8_START,
                >= 55 => MobDifficulty.HALL_7_END,
                >= 45 => MobDifficulty.HALL_7_MID,
                >= 40 => MobDifficulty.HALL_7_START,
                >= 35 => MobDifficulty.HALL_6_END,
                >= 25 => MobDifficulty.HALL_6_MID,
                >= 20 => MobDifficulty.HALL_6_START,
                >= 15 => MobDifficulty.HALL_5_END,
                _ => MobDifficulty.HALL_5_START
            };
        }

        private static MobDifficulty GetMinimumDifficultyByPlayerItems(Player player)
        {
            var invenory = player.inventory;

            var itemsToView = new List<InventoryItem>();
            itemsToView.AddRange(GetBestItems(invenory.GetItemsByType(ItemType.Sword), 1));
            itemsToView.AddRange(GetBestItems(invenory.GetItemsByType(ItemType.Bow), 1));
            itemsToView.AddRange(GetBestItems(invenory.GetItemsByType(ItemType.Stick), 1));
            itemsToView.AddRange(GetBestItems(invenory.GetItemsByType(ItemType.Helmet), 1));
            itemsToView.AddRange(GetBestItems(invenory.GetItemsByType(ItemType.Armor), 1));
            itemsToView.AddRange(GetBestItems(invenory.GetItemsByType(ItemType.Boots), 1));
            itemsToView.AddRange(GetBestItems(invenory.GetItemsByType(ItemType.Shield), 1));
            itemsToView.AddRange(GetBestItems(invenory.GetItemsByType(ItemType.Amulet), 1));
            itemsToView.AddRange(GetBestItems(invenory.GetItemsByType(ItemType.Ring), 2));
            itemsToView.AddRange(GetBestItems(invenory.GetItemsByType(ItemType.Scroll), 3));
            var averageInfo = GetAverageItemsInfo(itemsToView);

            switch (averageInfo.townHall)
            {
                case 8:
                    if (averageInfo.rarity >= Rarity.Legendary) return MobDifficulty.HALL_8_END;
                    if (averageInfo.rarity >= Rarity.Epic) return MobDifficulty.HALL_8_MID;
                    return MobDifficulty.HALL_8_START;
                case 7:
                    if (averageInfo.rarity >= Rarity.Legendary) return MobDifficulty.HALL_7_END;
                    if (averageInfo.rarity >= Rarity.Epic) return MobDifficulty.HALL_7_MID;
                    return MobDifficulty.HALL_7_START;
                case 6:
                    if (averageInfo.rarity >= Rarity.Legendary) return MobDifficulty.HALL_7_START; // Возможно стоит это убрать?
                    if (averageInfo.rarity >= Rarity.Epic) return MobDifficulty.HALL_6_END;
                    return MobDifficulty.HALL_6_START;
                case 5:
                    if (averageInfo.rarity >= Rarity.Legendary) return MobDifficulty.HALL_6_START; // Возможно стоит это убрать?
                    if (averageInfo.rarity >= Rarity.Epic) return MobDifficulty.HALL_5_END;
                    return MobDifficulty.HALL_5_START;
                case 4:
                    if (averageInfo.rarity >= Rarity.Epic) return MobDifficulty.HALL_4_END;
                    return MobDifficulty.HALL_4_START;

                default:
                    if (averageInfo.rarity >= Rarity.Epic) return MobDifficulty.HALL_3_END;
                    return MobDifficulty.HALL_3_START;
            }
        }

        private static List<InventoryItem> GetBestItems(IEnumerable<InventoryItem> items, int count)
        {
            var sortedItems = items.OrderByDescending(x => x.data.requiredTownHall).OrderByDescending(x => x.data.itemRarity).ToList();
            if (sortedItems.Count <= count)
            {
                return sortedItems;
            }

            var result = new List<InventoryItem>();
            for (int i = 0; i < count; i++)
            {
                result.Add(sortedItems[i]);
            }
            return result;
        }

        private static (byte townHall, Rarity rarity) GetAverageItemsInfo(List<InventoryItem> itemsToView)
        {
            int townHallSum = 0;
            int raritySum = 0;
            foreach (var item in itemsToView)
            {
                townHallSum += item.data.requiredTownHall;
                raritySum += (int)item.data.itemRarity;
            }
            var averageTownHall = (float)townHallSum / itemsToView.Count;
            var averageRarity = (float)raritySum / itemsToView.Count;

            var flooredTownHall = (int)averageTownHall;
            var flooredRarity = (int)averageRarity;

            var resultTownHall = averageTownHall - flooredTownHall > 0.65
                ? (byte)(flooredTownHall + 1)
                : (byte)flooredTownHall;

            var resultRarity = averageRarity - flooredRarity > 0.65
                ? (Rarity)(flooredRarity + 1)
                : (Rarity)flooredRarity;

            return (resultTownHall, resultRarity);
        }


    }
}
