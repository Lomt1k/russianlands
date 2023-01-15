using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.Bot.Sessions;
using System.Threading.Tasks;
using System.Text;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character
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
            await ShowCategories()
                .ConfigureAwait(false);
        }

        private async Task ShowCategories(CompareData? compareData = null)
        {
            var tooltip = session.tooltipController.TryGetTooltip(this);

            ClearButtons();
            RegisterCategoryButton(ItemType.Sword, 0);
            RegisterCategoryButton(ItemType.Bow, 1);
            RegisterCategoryButton(ItemType.Stick, 2);
            RegisterCategoryButton(ItemType.Helmet, 3);
            RegisterCategoryButton(ItemType.Armor, 4);
            RegisterCategoryButton(ItemType.Boots, 5);
            RegisterCategoryButton(ItemType.Shield, 6);
            RegisterCategoryButton(ItemType.Ring, 7);
            RegisterCategoryButton(ItemType.Amulet, 8);
            RegisterCategoryButton(ItemType.Scroll, 9);
            RegisterBackButton(() => new TownCharacterDialog(session).Start());
            RegisterTownButton(isDoubleBack: true);

            var sb = new StringBuilder();
            sb.AppendLine(Emojis.ButtonInventory + Localization.Get(session, "menu_item_inventory").Bold());
            var dialogHasTooltip = TryAppendTooltip(sb, tooltip);

            await SendDialogMessage(sb, GetKeyboardWithRowSizes(3, 3, 3, 3))
                .ConfigureAwait(false);
            if (!dialogHasTooltip)
            {
                _inspectorPanel.compareData = compareData;
                await _inspectorPanel.ShowMainInfo()
                    .ConfigureAwait(false);
            }
        }

        private void RegisterCategoryButton(ItemType itemType, int buttonId)
        {
            var inventory = session.player.inventory;
            var dialogHasTooltip = tooltip != null;
            var isTooltipButton = dialogHasTooltip && tooltip?.buttonId == buttonId;

            var prefix = inventory.HasNewInCategory(itemType) && !dialogHasTooltip ? Emojis.ElementWarningRed.ToString()
                : isTooltipButton ? string.Empty
                : itemType.GetEmoji().ToString() + ' ';
            var text = prefix + itemType.GetCategoryLocalization(session);
            RegisterButton(text, () => ShowCategory(itemType));
        }

        public async Task ShowCategory(ItemType category, int page = 0, CompareData? newCompareData = null)
        {
            _inspectorPanel.OnDialogClose(); // Чтобы убрать кнопку "Экипированное"
            if (newCompareData.HasValue)
            {
                _inspectorPanel.compareData = newCompareData.Value;
            }

            ClearButtons();
            RegisterBackButton(Localization.Get(session, "menu_item_inventory") + Emojis.ButtonInventory,
                () => ShowCategories(_inspectorPanel.compareData));
            RegisterDoubleBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
                () => new TownCharacterDialog(session).Start());

            var sb = new StringBuilder();
            sb.AppendLine(Emojis.ButtonInventory + Localization.Get(session, "menu_item_inventory").Bold());
            if (_inspectorPanel.compareData.HasValue)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "menu_item_compare_button_header"));
            }

            await SendDialogMessage(sb, GetOneLineKeyboard())
                .ConfigureAwait(false);
            await _inspectorPanel.ShowCategory(category, page)
                .ConfigureAwait(false);
        }

    }
}
