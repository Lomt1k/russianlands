using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character
{
    public class PotionsDialog : DialogBase
    {
        private PotionsDialogPanel _potionsPanel;
        private bool _backToBuilding;

        public PotionsDialog(GameSession session, bool backToBuilding = false) : base(session)
        {
            _backToBuilding = backToBuilding;
            _potionsPanel = new PotionsDialogPanel(this, 0);
            RegisterPanel(_potionsPanel);

            RegisterButton(Localization.Get(session, "dialog_potions_produce_button"),
                () => TryOpenProductionDialog());
            //TODO: Кнопка "Ускорить всё"

            if (backToBuilding)
            {
                RegisterBackButton(() => new BuildingInfoDialog(session, BuildingType.AlchemyLab.GetBuilding()).Start());
                return;
            }
            RegisterBackButton(() => new TownCharacterDialog(session).Start());
            RegisterTownButton(isFullBack: true);
        }

        public override async Task Start()
        {
            var header = $"<b>{Emojis.menuItems[MenuItem.Potions]} {Localization.Get(session, "menu_item_potions")}</b>";
            var keyboard = _backToBuilding
                ? GetMultilineKeyboard()
                : GetMultilineKeyboardWithDoubleBack();
            await SendDialogMessage(header, keyboard);
        }

        private async Task TryOpenProductionDialog()
        {
            //TODO
        }

    }
}
