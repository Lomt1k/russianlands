using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.Sessions;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Inventory
{
    public class InventoryDialog : DialogBase
    {
        private InventoryInspectorDialogPanel _inspectorPanel;

        public InventoryDialog(GameSession _session) : base(_session)
        {
            _inspectorPanel = new InventoryInspectorDialogPanel(this, 0);
            RegisterPanel(_inspectorPanel);
        }

        public override async Task Start()
        {
            RegisterBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
                () => new TownCharacterDialog(session).Start());
            RegisterTownButton(isDoubleBack: true);

            var header = Emojis.ButtonInventory + Localization.Get(session, "menu_item_inventory").Bold();
            await SendDialogMessage(header, GetOneLineKeyboard()).ConfigureAwait(false);
            await SendPanelsAsync().ConfigureAwait(false);
        }

    }
}
