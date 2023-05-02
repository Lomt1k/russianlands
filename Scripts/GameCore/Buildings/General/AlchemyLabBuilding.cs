using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Potions;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Potions;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Services.GameData;

namespace TextGameRPG.Scripts.GameCore.Buildings.General;

public class AlchemyLabBuilding : BuildingBase
{
    private static readonly GameDataHolder gameDataBase = Services.Services.Get<GameDataHolder>();

    public override BuildingId buildingId => BuildingId.AlchemyLab;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.potionWorkshopLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.potionWorkshopLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.potionWorkshopStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.potionWorkshopStartConstructionTime = startConstructionTime;
    }

    public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.Append(Localization.Get(session, $"building_{buildingId}_description"));

        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return sb.ToString();

        var levelInfo = (AlchemyLabLevelInfo)buildingData.levels[currentLevel - 1];

        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_types_of_potions_header"));
        var potionTypesAmount = GetPotionsForCurrentLevel(data).Count;
        sb.Append(Emojis.ButtonPotions + potionTypesAmount.ToString());

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_potions_in_battle_header"));
        var potionsInBattle = levelInfo.potionsInBattle;
        sb.Append(Emojis.ButtonPotions + potionsInBattle.ToString());

        return sb.ToString();
    }

    public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.Append(Localization.Get(session, $"building_{buildingId}_description"));

        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return sb.ToString();

        var currentLevelInfo = (AlchemyLabLevelInfo)buildingData.levels[currentLevel - 1];
        var nextLevelInfo = (AlchemyLabLevelInfo)buildingData.levels[currentLevel];

        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_types_of_potions_header"));
        var currentLevelAmount = GetPotionsForCurrentLevel(data).Count;
        var nextLevelAmount = GetPotionsForNextLevel(data).Count;
        var delta = nextLevelAmount - currentLevelAmount;
        sb.Append(Emojis.ButtonPotions + nextLevelAmount.ToString() + (delta > 0 ? $" (<i>+{delta}</i>)" : string.Empty));

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_potions_in_battle_header"));
        currentLevelAmount = currentLevelInfo.potionsInBattle;
        nextLevelAmount = nextLevelInfo.potionsInBattle;
        delta = nextLevelAmount - currentLevelAmount;
        sb.Append(Emojis.ButtonPotions + nextLevelAmount.ToString() + (delta > 0 ? $" (<i>+{delta}</i>)" : string.Empty));

        return sb.ToString();
    }

    public List<PotionData> GetPotionsForCurrentLevel(ProfileBuildingsData data)
    {
        var result = new List<PotionData>();
        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return result;

        foreach (var potionData in gameDataBase.potions.GetAllData())
        {
            if (potionData.workshopLevel == currentLevel)
            {
                result.Add(potionData);
            }
        }
        return result;
    }

    public List<PotionData> GetPotionsForNextLevel(ProfileBuildingsData data)
    {
        var result = new List<PotionData>();
        var nextLevel = GetCurrentLevel(data) + 1;

        foreach (var potionData in gameDataBase.potions.GetAllData())
        {
            if (potionData.workshopLevel == nextLevel)
            {
                result.Add(potionData);
            }
        }
        return result;
    }

    public override Dictionary<string, Func<Task>> GetSpecialButtons(GameSession session, ProfileBuildingsData data)
    {
        var result = new Dictionary<string, Func<Task>>();

        if (!IsUnderConstruction(data))
        {
            result.Add(Emojis.ButtonPotions + Localization.Get(session, "menu_item_potions") + $" ({session.player.potions.GetReadyPotionsCount()})",
                () => new PotionsDialog(session).Start());
        }

        return result;
    }

    public ResourceData GetCurrentCraftCost(ProfileBuildingsData data)
    {
        var level = GetCurrentLevel(data);
        if (level < 1)
        {
            return new ResourceData();
        }

        var levelInfo = (AlchemyLabLevelInfo)buildingData.levels[level - 1];
        return new ResourceData(ResourceId.Herbs, levelInfo.craftCostInHerbs);
    }

    public int GetCurrentCraftTimeInSeconds(ProfileBuildingsData data)
    {
        var level = GetCurrentLevel(data);
        if (level < 1)
            return 0;

        var levelInfo = (AlchemyLabLevelInfo)buildingData.levels[level - 1];
        return levelInfo.craftTime;
    }

    public ResourceData GetCraftCostForBuildingLevel(int level)
    {
        if (level < 1)
        {
            return new ResourceData();
        }
        var levelInfo = (AlchemyLabLevelInfo)buildingData.levels[level - 1];
        return new ResourceData(ResourceId.Herbs, levelInfo.craftCostInHerbs);
    }

    public int GetCurrentPotionsInBattle(ProfileBuildingsData data)
    {
        var level = GetCurrentLevel(data);
        if (level < 1)
            return 0;

        var levelInfo = (AlchemyLabLevelInfo)buildingData.levels[level - 1];
        return levelInfo.potionsInBattle;
    }

}
