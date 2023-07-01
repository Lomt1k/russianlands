using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FastTelegramBot.DataTypes.Keyboards;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Skills;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Character.Skills;

internal class SkillsDialog : DialogBase
{
    private readonly PlayerResources _resources;
    private readonly PlayerSkills _skills;

    public SkillsDialog(GameSession _session) : base(_session)
    {
        _resources = session.player.resources;
        _skills = session.player.skills;
    }

    public override async Task Start()
    {
        var sb = new StringBuilder();
        sb.AppendLine(_resources.GetFruitsView());

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "building_skills_upgrade_recipes"));
        foreach (var itemType in PlayerSkills.GetAllSkillTypes())
        {
            var requiredFruits = PlayerSkills.GetRequiredFruits(itemType);
            var isFirstFruit = true;
            foreach (var resourceId in requiredFruits)
            {
                if (!isFirstFruit)
                {
                    sb.Append(" +");
                }
                sb.Append(' ' + resourceId.GetEmoji().ToString());
                isFirstFruit = false;
            }
            sb.Append(Emojis.bigSpace + itemType.GetCategoryLocalization(session).Bold() + itemType.GetEmoji());
            sb.AppendLine();
        }

        ClearButtons();
        RegisterSkillButtons();
        RegisterBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
            () => new CharacterDialog(session).Start());
        RegisterTownButton(isDoubleBack: true);

        sb.AppendLine();
        if (_skills.IsAllSkillsMax())
        {
            sb.AppendLine(Localization.Get(session, "dialog_skills_all_skills_has_max_level"));
        }
        else if (IsElixirBuildingUnderConstruction())
        {
            var buildingName = (BuildingId.ElixirWorkshop.GetBuilding().GetNextLevelLocalizedName(session, session.profile.buildingsData) + ':').Bold();
            sb.AppendLine(buildingName);
            sb.AppendLine(Localization.Get(session, $"building_{BuildingId.ElixirWorkshop}_unavailable_under_construction"));
        }
        else
        {
            sb.AppendLine(Localization.Get(session, "dialog_skills_upgrade_select_skill"));
        }

        await SendDialogMessage(sb, GetSpecialKeyboard()).FastAwait();
    }

    private void RegisterSkillButtons()
    {
        if (_skills.IsAllSkillsMax())
            return;
        if (IsElixirBuildingUnderConstruction())
            return;

        foreach (var itemType in PlayerSkills.GetAllSkillTypes())
        {
            var availableUpgradesByFruits = GetAvailableSkillUpgradesByFruits(itemType);
            var availableUpgradesBySkillLimit = _skills.GetSkillLimit() - _skills.GetValue(itemType);
            var alailableUpgrades = (byte)Math.Min(availableUpgradesByFruits, availableUpgradesBySkillLimit);

            var emoji = alailableUpgrades > 0 ? Emojis.GetNumeric(alailableUpgrades) : itemType.GetEmoji();
            RegisterButton(emoji + itemType.GetCategoryLocalization(session),
                () => new UpgradeSkillDialog(session, itemType, alailableUpgrades).Start());
        }
    }

    private int GetAvailableSkillUpgradesByFruits(ItemType itemType)
    {
        var result = int.MaxValue;
        var requiredFruits = PlayerSkills.GetRequiredFruits(itemType);
        foreach (var resourceId in requiredFruits)
        {
            var resourceAmount = _resources.GetValue(resourceId);
            if (resourceAmount < result)
            {
                result = resourceAmount;
            }
        }
        return result;
    }

    private Dictionary<ResourceId, int> GetRequiredFruits(ItemType itemType)
    {
        var requiredFruits = PlayerSkills.GetRequiredFruits(itemType);
        return new Dictionary<ResourceId, int>
        {
            { requiredFruits[0], 1 },
            { requiredFruits[1], 1 },
            { requiredFruits[2], 1 },
        };
    }

    private bool IsElixirBuildingUnderConstruction()
    {
        return session.player.buildings.IsBuildingUnderConstruction(BuildingId.ElixirWorkshop);
    }

    private ReplyKeyboardMarkup GetSpecialKeyboard()
    {
        return buttonsCount > 2 ? GetKeyboardWithRowSizes(3, 3, 2, 2) : GetOneLineKeyboard();
    }

}
