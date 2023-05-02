using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Potions;

namespace MarkOne.Scripts.Bot.Dialogs.Town.Character.Potions;

public class PotionsDialog : DialogWithPanel
{
    private readonly PotionsDialogPanel _potionsPanel;
    public override DialogPanelBase DialogPanel => _potionsPanel;

    public PotionsDialog(GameSession session) : base(session)
    {
        _potionsPanel = new PotionsDialogPanel(this);
        RegisterBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
            () => new TownCharacterDialog(session).Start());
        RegisterTownButton(isDoubleBack: true);
    }

    public override async Task Start()
    {
        var header = Emojis.ButtonPotions + Localization.Get(session, "menu_item_potions").Bold();
        await SendDialogMessage(header.Bold(), GetOneLineKeyboard()).FastAwait();
        await _potionsPanel.Start().FastAwait();
    }

    public async Task StartWithTryCraft(PotionData potionData, int amount)
    {
        var header = Emojis.ButtonPotions + Localization.Get(session, "menu_item_potions").Bold();
        await SendDialogMessage(header.Bold(), GetOneLineKeyboard()).FastAwait();
        await _potionsPanel.TryCraft(potionData, amount).FastAwait();
    }

    public async Task StartWithSelectionAmountToCraft(PotionData potionData)
    {
        var header = Emojis.ButtonPotions + Localization.Get(session, "menu_item_potions").Bold();
        await SendDialogMessage(header.Bold(), GetOneLineKeyboard()).FastAwait();
        await _potionsPanel.ShowPotionsProductionAmountSelection(potionData).FastAwait();
    }

}
