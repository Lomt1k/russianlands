using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    public class InventoryItemComparisonDialog : DialogBase
    {
        private InventoryItem _item;
        private CompareData _compareData;

        public InventoryItemComparisonDialog(GameSession _session, InventoryItem item, CompareData compareData) : base(_session)
        {
            _item = item;
            _compareData = compareData;
        }

        public override async Task Start()
        {
            ClearButtons();

            RegisterButton(Localization.Get(session, "menu_item_compare_another_button"), () => SelectAnotherItemToCompare());
            RegisterButton(Localization.Get(session, "menu_item_compare_end_button"), () => EndComparison());

            await SendDialogMessage(_compareData.comparedItem.GetView(session), null);
            await SendDialogMessage(_item.GetView(session), GetMultilineKeyboard());
        }

        private async Task SelectAnotherItemToCompare()
        {
            await new InventoryDialog(session).ShowCategory(_compareData.categoryOnStartCompare, 0, _compareData);
        }

        private async Task EndComparison()
        {
            await new InventoryItemDialog(session, _compareData.comparedItem,
                _compareData.categoryOnStartCompare,
                _compareData.currentPageOnStartCompare).Start();
        }

    }
}
