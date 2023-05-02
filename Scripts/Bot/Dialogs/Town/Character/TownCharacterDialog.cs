using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Quests.MainQuest;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Inventory;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Potions;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Skills;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character;

public class TownCharacterDialog : DialogBase
{
    private int? _regenHealthMessageId;

    public TownCharacterDialog(GameSession _session) : base(_session)
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
        RegisterButton(potionsButton, () => TryShowPotionsDialog());

        var skillsEmoji = IsSkillsDialogAvailable() ? Emojis.ButtonSkills : Emojis.ElementLocked;
        RegisterButton(skillsEmoji + Localization.Get(session, "menu_item_skills"), () => TryShowSkillsDialog());

        RegisterButton(Emojis.AvatarMale + Localization.Get(session, "menu_item_avatar"), null);
        RegisterButton(Emojis.ButtonNameChange + Localization.Get(session, "menu_item_namechange"),
            () => new EnterNameDialog(session).Start());
        RegisterTownButton(isDoubleBack: false);
    }

    public override async Task Start()
    {
        session.player.healhRegenerationController.InvokeRegen();

        var sb = new StringBuilder();
        var isFullHealth = session.player.unitStats.isFullHealth;
        sb.AppendLine(session.player.GetFullUnitInfoView(session, isFullHealth));
        TryAppendTooltip(sb);

        await SendDialogMessage(sb, GetKeyboardWithRowSizes(1, 2, 2, 1)).FastAwait();

        if (!isFullHealth)
        {
            await SendHealthRegenMessage().FastAwait();
        }
    }

    private async Task TryShowPotionsDialog()
    {
        if (!IsPotionsDialogAvailable())
        {
            ClearButtons();
            var text = Localization.Get(session, "building_potions_alchemy_required");
            RegisterBackButton(() => new TownCharacterDialog(session).Start());
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
            RegisterBackButton(() => new TownCharacterDialog(session).Start());
            await SendDialogMessage(text, GetOneLineKeyboard());
            return;
        }
        await new SkillsDialog(session).Start().FastAwait();
    }

    private bool IsSkillsDialogAvailable()
    {
        return session.player.IsSkillsAvailable();
    }

    private async Task SendHealthRegenMessage()
    {
        try
        {
            var sb = new StringBuilder();
            var stats = session.player.unitStats;
            if (_regenHealthMessageId.HasValue)
            {
                if (session.currentDialog != this)
                {
                    await messageSender.DeleteMessage(session.chatId, _regenHealthMessageId.Value).FastAwait();
                    return;
                }
                else if (stats.currentHP >= stats.maxHP)
                {
                    sb.AppendLine(Localization.Get(session, "unit_view_health"));
                    sb.AppendLine(Emojis.StatHealth + $"{stats.currentHP} / {stats.maxHP}");
                    await messageSender.EditTextMessage(session.chatId, _regenHealthMessageId.Value, sb.ToString(), cancellationToken: session.cancellationToken).FastAwait();
                    return;
                }
            }

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "unit_view_health_regen"));
            sb.AppendLine(Emojis.StatHealth + $"{stats.currentHP} / {stats.maxHP}");

            var message = _regenHealthMessageId == null
                ? await messageSender.SendTextMessage(session.chatId, sb.ToString(), silent: true, cancellationToken: session.cancellationToken).FastAwait()
                : await messageSender.EditTextMessage(session.chatId, _regenHealthMessageId.Value, sb.ToString(), cancellationToken: session.cancellationToken).FastAwait();
            _regenHealthMessageId = message?.MessageId;

            WaitOneSecondAndInvokeHealthRegen();
        }
        catch (System.Exception) { } //ignored
    }

    private async void WaitOneSecondAndInvokeHealthRegen()
    {
        try
        {
            await Task.Delay(1_000).FastAwait();
            if (session.IsTasksCancelled())
                return;

            session.player.healhRegenerationController.InvokeRegen();
            await SendHealthRegenMessage().FastAwait();
        }
        catch (System.Exception) { } //ignored
    }

}
