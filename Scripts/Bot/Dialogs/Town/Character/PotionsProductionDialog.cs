using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character
{
    public class PotionsProductionDialog : DialogBase
    {
        private PotionsProductionDialogPanel _productionPanel;

        public PotionsProductionDialog(GameSession session) : base(session)
        {
            _productionPanel = new PotionsProductionDialogPanel(this, 0);
            RegisterPanel(_productionPanel);
            RegisterBackButton($"{Localization.Get(session, "menu_item_potions")} {Emojis.menuItems[MenuItem.Potions]}",
                () => new PotionsDialog(session).Start());
            RegisterDoubleBackButton($"{Localization.Get(session, "menu_item_character")} {Emojis.characters[CharIcon.Male]}",
                () => new TownCharacterDialog(session).Start());
        }

        public override async Task Start()
        {
            var text = $"<b>{Localization.Get(session, "dialog_potions_produce_button")}</b>";
            await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
            await SendPanelsAsync().ConfigureAwait(false);
        }
    }
}
