using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Potions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Potions
{
    public class PotionsDialog : DialogBase
    {
        private PotionsDialogPanel _potionsPanel;

        public PotionsDialog(GameSession session) : base(session)
        {
            _potionsPanel = new PotionsDialogPanel(this, 0);
            RegisterPanel(_potionsPanel);

            RegisterBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
                () => new TownCharacterDialog(session).Start());
            RegisterTownButton(isDoubleBack: true);
        }

        public override async Task Start()
        {
            var header = Emojis.ButtonPotions + Localization.Get(session, "menu_item_potions").Bold();
            await SendDialogMessage(header.Bold(), GetOneLineKeyboard())
                .ConfigureAwait(false);
            await SendPanelsAsync()
                .ConfigureAwait(false);
        }

        public async Task StartWithTryCraft(PotionData potionData, int amount)
        {
            var header = Emojis.ButtonPotions + Localization.Get(session, "menu_item_potions").Bold();
            await SendDialogMessage(header.Bold(), GetOneLineKeyboard())
                .ConfigureAwait(false);
            await _potionsPanel.TryCraft(potionData, amount)
                .ConfigureAwait(false);
        }

        public async Task StartWithSelectionAmountToCraft(PotionData potionData)
        {
            var header = Emojis.ButtonPotions + Localization.Get(session, "menu_item_potions").Bold();
            await SendDialogMessage(header.Bold(), GetOneLineKeyboard())
                .ConfigureAwait(false);
            await _potionsPanel.ShowPotionsProductionAmountSelection(potionData)
                .ConfigureAwait(false);
        }

    }
}
