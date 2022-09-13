using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Buildings
{
    public class BuildingsDialog : DialogBase
    {
        private BuildingsInspectorPanel _inspectorPanel;

        public BuildingsDialog(GameSession _session) : base(_session)
        {
            _inspectorPanel = new BuildingsInspectorPanel(this, 0);
            RegisterPanel(_inspectorPanel);

            RegisterButton(BuildingCategory.General.GetLocalization(session),
                null);
            RegisterButton(BuildingCategory.Storages.GetLocalization(session),
                null);
            RegisterButton(BuildingCategory.Production.GetLocalization(session),
                null);
            RegisterButton(BuildingCategory.Training.GetLocalization(session),
                null);
            RegisterButton($"{Emojis.elements[Element.Back]} " + Localization.Get(session, "menu_item_back_button"),
                () => new TownEntryDialog(session, TownEntryReason.BackFromInnerDialog).Start());
        }

        public override async Task Start()
        {
            var header = $"{Emojis.menuItems[MenuItem.Buildings]} " + "<b>" + Localization.Get(session, "menu_item_buildings") + "</b>";
            await messageSender.SendTextDialog(session.chatId, header, GetKeyboardWithRowSizes(2, 2, 1));
            await SendPanelsAsync();
        }
    }
}
