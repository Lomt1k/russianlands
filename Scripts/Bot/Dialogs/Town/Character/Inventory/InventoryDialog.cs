using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.Sessions;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Inventory
{
    public class InventoryDialog : DialogWithPanel
    {
        private InventoryInspectorDialogPanel _inspectorPanel;
        public override DialogPanelBase DialogPanel => _inspectorPanel;

        public InventoryDialog(GameSession _session) : base(_session)
        {
            _inspectorPanel = new InventoryInspectorDialogPanel(this);
            RegisterBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
                () => new TownCharacterDialog(session).Start());
            RegisterTownButton(isDoubleBack: true);
        }

        public override async Task Start()
        {
            var header = Emojis.ButtonInventory + Localization.Get(session, "menu_item_inventory").Bold();
            await SendDialogMessage(header, GetOneLineKeyboard()).ConfigureAwait(false);
            await _inspectorPanel.Start().ConfigureAwait(false);
        }

    }
}
