using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localization;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    internal class InventoryInspectorDialogPanel : DialogPanelBase
    {
        private readonly string _mainItemsInfo;

        private PlayerInventory _inventory;
        private Message? _lastMessage;

        private ItemType? _browsedCategory;
        private InventoryItem[]? _browsedItems;
        

        public InventoryInspectorDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
            _inventory = session.player.inventory;
            _mainItemsInfo = BuildMainItemsInfo();
        }

        private string BuildMainItemsInfo()
        {
            var sb = new StringBuilder();

            sb.Append($"{Emojis.items[ItemType.Sword]} {_inventory.GetItemsCountByType(ItemType.Sword)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Bow]} {_inventory.GetItemsCountByType(ItemType.Bow)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Stick]} {_inventory.GetItemsCountByType(ItemType.Stick)}");


            sb.AppendLine();
            sb.Append($"{Emojis.items[ItemType.Helmet]} {_inventory.GetItemsCountByType(ItemType.Helmet)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Armor]} {_inventory.GetItemsCountByType(ItemType.Armor)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Boots]} {_inventory.GetItemsCountByType(ItemType.Boots)}");

            sb.AppendLine();
            sb.Append($"{Emojis.items[ItemType.Shield]} {_inventory.GetItemsCountByType(ItemType.Shield)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Amulet]} {_inventory.GetItemsCountByType(ItemType.Amulet)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Ring]} {_inventory.GetItemsCountByType(ItemType.Ring)}");

            sb.AppendLine();
            sb.Append($"{Emojis.items[ItemType.Poison]} {_inventory.GetItemsCountByType(ItemType.Poison)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Tome]} {_inventory.GetItemsCountByType(ItemType.Tome)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Scroll]} {_inventory.GetItemsCountByType(ItemType.Scroll)}");

            return sb.ToString();
        }

        public override async Task SendAsync()
        {
            await ShowMainInfo();
        }

        public async Task ShowMainInfo()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine(Localization.Get(session, "dialog_inventory_header_all_items"));
            text.AppendLine();
            text.Append(_mainItemsInfo);

            if (_lastMessage?.ReplyMarkup != null)
            {
                await messageSender.EditMessageKeyboard(session.chatId, _lastMessage.MessageId, null);
            }
            _lastMessage = await messageSender.SendTextMessage(session.chatId, text.ToString(), GetMultilineKeyboard());
        }

        public async Task ShowCategory(ItemType category)
        {
            if (_lastMessage == null)
                return;

            if (_lastMessage.ReplyMarkup != null)
            {
                await messageSender.EditMessageKeyboard(session.chatId, _lastMessage.MessageId, null);
            }
            
            _browsedCategory = category;
            _browsedItems = _inventory.GetItemsByType(category).ToArray();

            if (_browsedItems.Length == 0)
            {
                string stringCategory = category.ToString().ToLower();
                if (!stringCategory.EndsWith('s'))
                {
                    stringCategory = stringCategory + 's';
                }

                var text = $"<b>{Localization.Get(session, $"menu_item_{stringCategory}")}:</b> "
                    + Localization.Get(session, "dialog_inventory_has_not_items");
                _lastMessage = await messageSender.SendTextMessage(session.chatId, text);
                return;
            }
            await ShowItemsList(0, asNewMessage: true);
        }

        private async Task ShowItemsList(int startIndex, bool asNewMessage)
        {
            //TODO
        }

        public override void OnDialogClose()
        {
        }

    }
}
