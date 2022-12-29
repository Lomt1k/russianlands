using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Shop;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Potions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character
{
    public class PotionsDialog : DialogBase
    {
        private PotionsDialogPanel _potionsPanel;

        public PotionsDialog(GameSession session) : base(session)
        {
            _potionsPanel = new PotionsDialogPanel(this, 0);
            RegisterPanel(_potionsPanel);

            RegisterButton($"{Emojis.elements[Element.Plus]} " + Localization.Get(session, "dialog_potions_produce_button"),
                () => TryOpenProductionDialog());
            //TODO: Кнопка "Ускорить всё"

            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_character")} {Emojis.characters[CharIcon.Male]}",
                () => new TownCharacterDialog(session).Start());
            RegisterTownButton(isFullBack: true);
        }

        public override async Task Start()
        {
            var header = $"<b>{Emojis.menuItems[MenuItem.Potions]} {Localization.Get(session, "menu_item_potions")}</b>";
            await SendDialogMessage(header, GetMultilineKeyboardWithDoubleBack());
            await SendPanelsAsync();
        }

        private async Task TryOpenProductionDialog()
        {
            bool isFull = session.player.potions.IsFull(session);
            if (isFull)
            {
                ClearButtons();
                var sb = new StringBuilder();
                sb.AppendLine(Localization.Get(session, "dialog_potions_limit"));

                if (!session.profile.data.IsPremiumActive())
                {
                    sb.AppendLine();
                    sb.AppendLine(Localization.Get(session, "dialog_potions_limit_can_buy_premium"));
                    RegisterButton($"{Emojis.menuItems[MenuItem.Shop]} {Localization.Get(session, "menu_item_shop")}", () => new ShopDialog(session).Start());
                }

                RegisterBackButton(() => new PotionsDialog(session).Start());
                await SendDialogMessage(sb, GetMultilineKeyboard());
                return;
            }

            await new PotionsProductionDialog(session).Start();
        }

    }
}
