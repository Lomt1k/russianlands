using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Dialogs.Town.Character.Skills;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Buildings.General;

public class ElixirWorkshopBuilding : BuildingBase
{
    public override BuildingId buildingId => BuildingId.ElixirWorkshop;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.elixirWorkshopLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.elixirWorkshopLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.elixirWorkshopStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.elixirWorkshopStartConstructionTime = startConstructionTime;
    }

    public override Dictionary<string, Func<Task>> GetSpecialButtons(GameSession session, ProfileBuildingsData data)
    {
        var result = new Dictionary<string, Func<Task>>();

        if (!IsUnderConstruction(data))
        {
            result.Add(Emojis.ButtonSkills + Localization.Get(session, "menu_item_skills"),
                () => new SkillsDialog(session).Start());
        }

        return result;
    }

    public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));

        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return sb.ToString();

        sb.AppendLine();
        var maxSkillLevel = GetCurrentMaxSkillLevel(data);
        var formatted = Localization.Get(session, "building_skills_limit", maxSkillLevel);
        sb.Append(Emojis.ButtonSkills + formatted);

        return sb.ToString();
    }

    public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, $"building_{buildingId}_description"));
        sb.AppendLine();

        var currentValue = GetCurrentMaxSkillLevel(data);
        var nextValue = GetNextMaxSkillLevel(data);
        var delta = nextValue - currentValue;
        var hideDelta = !IsBuilt(data);
        var dynamicData = nextValue + (hideDelta ? string.Empty : $" (<i>+{delta}</i>)");
        var formatted = Localization.Get(session, "building_skills_limit", dynamicData);
        sb.Append(Emojis.ButtonSkills + formatted);
        return sb.ToString();
    }

    public int GetCurrentMaxSkillLevel(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
            return 0;

        var levelInfo = (ElixirWorkshopLevelInfo)buildingData.levels[currentLevel - 1];
        return levelInfo.skillLevelLimit;
    }

    public int GetNextMaxSkillLevel(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        var levelInfo = (ElixirWorkshopLevelInfo)buildingData.levels[currentLevel];
        return levelInfo.skillLevelLimit;
    }

    public ResourceData GetCurrentElixirPriceInHerbs(ProfileBuildingsData data)
    {
        var currentLevel = GetCurrentLevel(data);
        if (currentLevel < 1)
        {
            return new ResourceData(ResourceId.Herbs, 0);
        }

        var levelInfo = (ElixirWorkshopLevelInfo)buildingData.levels[currentLevel - 1];
        return new ResourceData(ResourceId.Herbs, levelInfo.elixirPriceInHerbs);
    }

}
