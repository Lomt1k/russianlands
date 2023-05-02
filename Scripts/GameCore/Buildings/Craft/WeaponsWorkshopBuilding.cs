using System;
using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;

namespace MarkOne.Scripts.GameCore.Buildings.Craft;

public class WeaponsWorkshopBuilding : CraftBuildingBase
{
    public override List<ItemType> craftCategories => new List<ItemType>
    {
        ItemType.Sword,
        ItemType.Bow,
        ItemType.Stick
    };

    public override BuildingId buildingId => BuildingId.WeaponsWorkshop;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.weaponsWorkshopLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.weaponsWorkshopLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.weaponsWorkshopStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.weaponsWorkshopStartConstructionTime = startConstructionTime;
    }

    public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.Append(Localization.Get(session, "building_WeaponsWorkshop_description"));

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
        sb.Append(Localization.Get(session, "building_WeaponsWorkshop_description"));

        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_craft_items_level_header"));
        var itemLevels = GetNextCraftLevels(data);
        sb.Append(itemLevels);

        return sb.ToString();
    }

    public override DateTime GetStartCraftTime(ProfileBuildingsData data)
    {
        return data.weaponsWorkshopStartCraftTime;
    }

    protected override void SetStartCraftTime(ProfileBuildingsData data, DateTime startCraftTime)
    {
        data.weaponsWorkshopStartCraftTime = startCraftTime;
    }

    public override ItemType GetCurrentCraftItemType(ProfileBuildingsData data)
    {
        return data.weaponsWorkshopCraftItemType;
    }

    protected override void SetCurrentCraftItemType(ProfileBuildingsData data, ItemType itemType)
    {
        data.weaponsWorkshopCraftItemType = itemType;
    }

    public override Rarity GetCurrentCraftItemRarity(ProfileBuildingsData data)
    {
        return data.weaponsWorkshopCraftItemRarity;
    }

    protected override void SetCurrentCraftItemRarity(ProfileBuildingsData data, Rarity rarity)
    {
        data.weaponsWorkshopCraftItemRarity = rarity;
    }

}
