using System;
using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;

namespace MarkOne.Scripts.GameCore.Buildings.Craft;

public class ArmorWorkshopBuilding : CraftBuildingBase
{
    public override List<ItemType> craftCategories => new List<ItemType>
    {
        ItemType.Armor,
        ItemType.Helmet,
        ItemType.Boots,
        ItemType.Shield
    };

    public override BuildingId buildingId => BuildingId.ArmorWorkshop;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.armorWorkshopLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.armorWorkshopLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.armorWorkshopStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
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

    public override DateTime GetStartCraftTime(ProfileBuildingsData data)
    {
        return data.armorWorkshopStartCraftTime;
    }

    protected override void SetStartCraftTime(ProfileBuildingsData data, DateTime startCraftTime)
    {
        data.armorWorkshopStartCraftTime = startCraftTime;
    }

    public override ItemType GetCurrentCraftItemType(ProfileBuildingsData data)
    {
        return data.armorWorkshopCraftItemType;
    }

    protected override void SetCurrentCraftItemType(ProfileBuildingsData data, ItemType itemType)
    {
        data.armorWorkshopCraftItemType = itemType;
    }

    public override Rarity GetCurrentCraftItemRarity(ProfileBuildingsData data)
    {
        return data.armorWorkshopCraftItemRarity;
    }

    protected override void SetCurrentCraftItemRarity(ProfileBuildingsData data, Rarity rarity)
    {
        data.armorWorkshopCraftItemRarity = rarity;
    }

}
