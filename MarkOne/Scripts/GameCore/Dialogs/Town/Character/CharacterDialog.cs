using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Dialogs.Quests.MainQuest;
using MarkOne.Scripts.GameCore.Dialogs.Town.Character.Inventory;
using MarkOne.Scripts.GameCore.Dialogs.Town.Character.Potions;
using MarkOne.Scripts.GameCore.Dialogs.Town.Character.Skills;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Character;

public class CharacterDialog : DialogBase
{
    public CharacterDialog(GameSession _session) : base(_session)
    {
        var player = session.player;
        var hasTooltip = session.tooltipController.HasTooltipToAppend(this);

        var inventoryButton = Emojis.ButtonInventory + Localization.Get(session, "menu_item_inventory")
            + (player.inventory.hasAnyNewItem && !hasTooltip ? Emojis.ElementWarningRed.ToString() : string.Empty);
        RegisterButton(inventoryButton, () => new InventoryDialog(session).Start());

        var potionsText = Localization.Get(session, "menu_item_potions");
        var potionsButton = IsPotionsDialogAvailable()
            ? Emojis.ButtonPotions + potionsText + $" ({session.player.potions.GetReadyPotionsCount()})"
            : Emojis.ElementLocked + potionsText;
        RegisterButton(potionsButton, TryShowPotionsDialog);

        var skillsEmoji = IsSkillsDialogAvailable() ? Emojis.ButtonSkills : Emojis.ElementLocked;
        RegisterButton(skillsEmoji + Localization.Get(session, "menu_item_skills"), TryShowSkillsDialog);

        RegisterButton(Emojis.AvatarMale + Localization.Get(session, "menu_item_avatar"), null);
        RegisterButton(Emojis.ButtonNameChange + Localization.Get(session, "menu_item_namechange"),
            () => new EnterNameDialog(session).Start());
        RegisterTownButton(isDoubleBack: false);
    }

    public override async Task Start()
    {
        session.player.healhRegenerationController.InvokeRegen();

        var sb = new StringBuilder();
        sb.AppendLine(session.player.GetFullUnitInfoView(session));
        TryAppendTooltip(sb);

        await SendDialogMessage(sb, GetKeyboardWithRowSizes(1, 2, 2, 1)).FastAwait();
    }

    private async Task TryShowPotionsDialog()
    {
        if (!IsPotionsDialogAvailable())
        {
            ClearButtons();
            var text = Localization.Get(session, "building_potions_alchemy_required");
            RegisterBackButton(() => new CharacterDialog(session).Start());
            await SendDialogMessage(text, GetOneLineKeyboard());
            return;
        }
        await new PotionsDialog(session).Start().FastAwait();
    }

    private bool IsPotionsDialogAvailable()
    {
        return session.player.buildings.HasBuilding(BuildingId.AlchemyLab);
    }

    private async Task TryShowSkillsDialog()
    {
        if (!IsSkillsDialogAvailable())
        {
            ClearButtons();
            var text = Localization.Get(session, "building_skills_elixir_workshop_required");
            RegisterBackButton(() => new CharacterDialog(session).Start());
            await SendDialogMessage(text, GetOneLineKeyboard());
            return;
        }
        await new SkillsDialog(session).Start().FastAwait();
    }

    private bool IsSkillsDialogAvailable()
    {
        return session.player.IsSkillsAvailable();
    }

}
