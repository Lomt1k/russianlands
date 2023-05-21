using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.General;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Skills;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Dialogs.Resources;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Character.Skills;

internal class UpgradeSkillDialog : DialogBase
{
    private readonly PlayerResources _resources;
    private readonly PlayerSkills _skills;
    private readonly ItemType _itemType;
    private readonly byte _upgradeButtons;

    public UpgradeSkillDialog(GameSession _session, ItemType itemType, byte availableUpgrades) : base(_session)
    {
        _resources = session.player.resources;
        _skills = session.player.skills;
        _itemType = itemType;
        _upgradeButtons = Math.Min(availableUpgrades, (byte)10);

        for (byte i = 1; i <= _upgradeButtons; i++)
        {
            var amountForDelegate = i; //it is important!
            RegisterButton(i.ToString(), () => TryUpgrade(amountForDelegate));
        }
        RegisterBackButton(() => new SkillsDialog(session).Start());
    }

    public override async Task Start()
    {
        var sb = new StringBuilder();
        sb.AppendLine(_itemType.GetEmoji() + _itemType.GetCategoryLocalization(session).Bold());
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "dialog_skills_upgrade_selected_skill_description"));

        sb.AppendLine();
        sb.Append(GetRequiredResources().GetPriceView(session));

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "resource_header_ours"));
        var requiredFruits = PlayerSkills.GetRequiredFruits(_itemType);
        var ourResources = new ResourceData[]
        {
            new ResourceData(requiredFruits[0], _resources.GetValue(requiredFruits[0])),
            new ResourceData(requiredFruits[1], _resources.GetValue(requiredFruits[1])),
            new ResourceData(requiredFruits[2], _resources.GetValue(requiredFruits[2])),
            new ResourceData(ResourceId.Herbs, session.player.resources.GetValue(ResourceId.Herbs)),
        };
        sb.Append(ourResources.GetLocalizedView(session));

        sb.AppendLine();
        if (_skills.IsMaxLevel(_itemType))
        {
            sb.AppendLine(Localization.Get(session, "dialog_skills_upgrade_skill_has_max_level"));
        }
        else if (!_resources.HasEnough(GetRequiredFruits()))
        {
            sb.AppendLine(Emojis.ElementWarningGrey.ToString() + Localization.Get(session, "dialog_skills_upgrade_skill_no_upgrades_available"));
        }
        else
        {
            sb.Append(Localization.Get(session, "dialog_skills_upgrade_elixirs_amount"));
        }

        await SendDialogMessage(sb, GetSpecialKeyboard()).FastAwait();
    }

    private ResourceData[] GetRequiredResources()
    {
        var elixirWorkshop = (ElixirWorkshopBuilding)BuildingId.ElixirWorkshop.GetBuilding();
        var requiredFruits = PlayerSkills.GetRequiredFruits(_itemType);
        return new ResourceData[]
        {
            new ResourceData(requiredFruits[0], 1),
            new ResourceData(requiredFruits[1], 1),
            new ResourceData(requiredFruits[2], 1),
            elixirWorkshop.GetCurrentElixirPriceInHerbs(session.profile.buildingsData),
        };
    }

    private ResourceData[] GetRequiredFruits()
    {
        var requiredFruits = PlayerSkills.GetRequiredFruits(_itemType);
        return new ResourceData[]
        {
            new ResourceData(requiredFruits[0], 1),
            new ResourceData(requiredFruits[1], 1),
            new ResourceData(requiredFruits[2], 1),
        };
    }

    private ReplyKeyboardMarkup GetSpecialKeyboard()
    {
        return _upgradeButtons > 5
            ? GetKeyboardWithRowSizes(5, _upgradeButtons - 5, 1)
            : GetKeyboardWithRowSizes(_upgradeButtons, 1);
    }

    private async Task TryUpgrade(byte levelsCount)
    {
        var requiredResources = GetRequiredResources();
        for (var i = 0; i < requiredResources.Length; i++)
        {
            requiredResources[i].amount *= levelsCount;
        }

        var playerResources = session.player.resources;
        var successfullPurchase = playerResources.TryPurchase(requiredResources, out var notEnoughResources);
        if (successfullPurchase)
        {
            await SkillUp(levelsCount).FastAwait();
            return;
        }

        var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, notEnoughResources,
            onSuccess: async () => await new UpgradeSkillDialog(session, _itemType, _upgradeButtons).TryUpgrade(levelsCount).FastAwait(),
            onCancel: async () => await new UpgradeSkillDialog(session, _itemType, _upgradeButtons).Start().FastAwait());
        await buyResourcesDialog.Start().FastAwait();
    }

    private async Task SkillUp(byte amount)
    {
        _skills.AddValue(_itemType, amount);

        var sb = new StringBuilder();
        sb.AppendLine(_itemType.GetEmoji() + _itemType.GetCategoryLocalization(session).Bold());
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "dialog_skills_upgrade_successfull", _skills.GetValue(_itemType)));

        ClearButtons();
        RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => new SkillsDialog(session).Start());

        await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
    }

}
