using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.Bot.Dialogs.Town.Buildings.CraftBuildingDialog;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Items.Generators;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.GameCore.Buildings;

public struct CraftItemLevelsInfo
{
    public int minLevel;
    public int maxLevel;

    public override string ToString()
    {
        return $"{minLevel} - {maxLevel}";
    }
}

public abstract class CraftBuildingBase : BuildingBase
{
    public abstract List<ItemType> craftCategories { get; }

    public abstract DateTime GetStartCraftTime(ProfileBuildingsData data);
    protected abstract void SetStartCraftTime(ProfileBuildingsData data, DateTime startCraftTime);
    public abstract ItemType GetCurrentCraftItemType(ProfileBuildingsData data);
    protected abstract void SetCurrentCraftItemType(ProfileBuildingsData data, ItemType itemType);
    public abstract Rarity GetCurrentCraftItemRarity(ProfileBuildingsData data);
    protected abstract void SetCurrentCraftItemRarity(ProfileBuildingsData data, Rarity rarity);

    /// <returns>Ведётся ли сейчас изготовление предмета</returns>
    public bool IsCraftStarted(ProfileBuildingsData data)
    {
        return GetStartCraftTime(data) > DateTime.MinValue;
    }

    /// <returns>Время изготовления предмета в зависимости от его редкости</returns>
    public ResourceData[] GetCraftPrice(ProfileBuildingsData data, Rarity rarity)
    {
        var result = new ResourceData[2];

        var currentLevel = GetCurrentLevel(data);
        var levelInfo = (CraftLevelInfo)buildingData.levels[currentLevel - 1];
        switch (rarity)
        {
            case Rarity.Rare:
                result[0] = new ResourceData(ResourceId.CraftPiecesCommon, levelInfo.rareCraft_MaterialsCost);
                result[1] = new ResourceData(ResourceId.Wood, levelInfo.rareCraft_WoodCost);
                break;
            case Rarity.Epic:
                result[0] = new ResourceData(ResourceId.CraftPiecesRare, levelInfo.epicCraft_MaterialsCost);
                result[1] = new ResourceData(ResourceId.Wood, levelInfo.epicCraft_WoodCost);
                break;
            case Rarity.Legendary:
                result[0] = new ResourceData(ResourceId.CraftPiecesEpic, levelInfo.legendaryCraft_MaterialsCost);
                result[1] = new ResourceData(ResourceId.Wood, levelInfo.legendaryCraft_WoodCost);
                break;
        }
        return result;
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
        var startDt = GetStartCraftTime(data);
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
        return EndOfCraftIsImportant(data) && IsCraftCanBeFinished(data);
    }

    protected override List<string> GetUpdatesInternal(GameSession session, ProfileBuildingsData data, bool onlyImportant)
    {
        var updates = new List<string>();

        if (IsCraftStarted(data))
        {
            if (IsCraftCanBeFinished(data))
            {
                // первое сообщение о конце крафта считается important, далее - нет
                var endCraftIsImprortant = EndOfCraftIsImportant(data);
                if (endCraftIsImprortant)
                {
                    MarkEndOfCraftAsNotImportant(data);
                }
                if (endCraftIsImprortant || !onlyImportant)
                {
                    updates.Add(Localization.Get(session, "dialog_craft_completed"));
                }
            }
            else if (!onlyImportant)
            {
                var sb = new StringBuilder();
                var timeSpan = GetEndCraftTime(data) - DateTime.UtcNow;
                var productionView = Localization.Get(session, "dialog_craft_progress", timeSpan.GetView(session));
                sb.AppendLine(productionView);

                var itemType = GetCurrentCraftItemType(data);
                sb.Append(itemType.GetEmoji() + itemType.GetLocalization(session).Bold());
                updates.Add(sb.ToString());
            }
        }

        return updates;
    }

    // первое сообщение о конце крафта считается important, далее - нет
    private bool EndOfCraftIsImportant(ProfileBuildingsData data)
    {
        return GetStartCraftTime(data).Ticks > 1;
    }

    private void MarkEndOfCraftAsNotImportant(ProfileBuildingsData data)
    {
        SetStartCraftTime(data, new DateTime(1));
    }

    public override Dictionary<string, Func<Task>> GetSpecialButtons(GameSession session, ProfileBuildingsData data)
    {
        var result = new Dictionary<string, Func<Task>>();

        if (!IsUnderConstruction(data))
        {
            var buttonText = Emojis.ButtonCraft + Localization.Get(session, "menu_item_craft");
            if (IsCraftStarted(data))
            {
                if (IsCraftCanBeFinished(data))
                {
                    result.Add(buttonText, () => new CraftCanCollectItemDialog(session, this).Start());
                }
                else
                {
                    result.Add(buttonText, () => new CraftInProgressDialog(session, this).Start());
                }
            }
            else
            {
                result.Add(buttonText, () => new CraftNewItemDialog(session, this).Start());
            }
        }

        return result;
    }

    public CraftItemLevelsInfo GetCurrentCraftLevels(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
        {
            return new CraftItemLevelsInfo() { minLevel = 0, maxLevel = 0 };
        }

        var townhallLevel = buildingData.levels[currentLevel - 1].requiredTownHall;
        return new CraftItemLevelsInfo()
        {
            minLevel = ItemGenerationHelper.CalculateRequiredLevel(townhallLevel, 1),
            maxLevel = ItemGenerationHelper.CalculateRequiredLevel(townhallLevel, 10)
        };
    }

    public CraftItemLevelsInfo GetNextCraftLevels(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        var townhallLevel = buildingData.levels[currentLevel].requiredTownHall;

        return new CraftItemLevelsInfo()
        {
            minLevel = ItemGenerationHelper.CalculateRequiredLevel(townhallLevel, 1),
            maxLevel = ItemGenerationHelper.CalculateRequiredLevel(townhallLevel, 10)
        };
    }

    public void StartCraft(ProfileBuildingsData data, ItemType itemType, Rarity rarity)
    {
        SetCurrentCraftItemType(data, itemType);
        SetCurrentCraftItemRarity(data, rarity);
        SetStartCraftTime(data, DateTime.UtcNow);
    }

    public void BoostCraft(ProfileBuildingsData data)
    {
        SetStartCraftTime(data, new DateTime(1));
    }

    public InventoryItem GetCraftItemAndResetCraft(ProfileBuildingsData data)
    {
        SetStartCraftTime(data, DateTime.MinValue);
        var itemType = GetCurrentCraftItemType(data);
        var rarity = GetCurrentCraftItemRarity(data);
        var townhallLevel = GetCurrentTownhallLevelForCraftItem(data);
        var item = ItemGenerationManager.GenerateItem(townhallLevel, itemType, rarity);
        return item;
    }

    public int GetCurrentTownhallLevelForCraftItem(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
        {
            return 1;
        }
        return buildingData.levels[currentLevel - 1].requiredTownHall;
    }

    public override bool IsStartConstructionBlocked(ProfileBuildingsData data, out string blockReasonMessage)
    {
        var isBlocked = IsCraftStarted(data);
        blockReasonMessage = isBlocked
            ? Localization.Get(data.session, "dialog_buildings_construction_blocked_because_craft")
            : string.Empty;
        return isBlocked;
    }

}
