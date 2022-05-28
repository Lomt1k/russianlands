using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Commands
{
    internal class AddItemCommand : CommandBase
    {
        public override async void Execute(GameSession session, string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
                return;

            if (!int.TryParse(args[0], out var itemId))
                return;

            int count = 1;
            if (args.Length >= 2 && !int.TryParse(args[1], out count))
                return;

            int result = 0;
            InventoryItem? firstAddedItem = null;
            for (int i = 0; i < count; i++)
            {
                var addedItem = session.player.inventory.TryAddItem(itemId);
                if (addedItem == null)
                    break;

                result++;
                if (result == 1)
                    firstAddedItem = addedItem;
            }

            if (result == 0)
                return;

            string message = $"Successfully added item '{firstAddedItem.data.debugName}' (Count: {result})";
            await messageSender.SendTextMessage(session.chatId, message);
        }
    }
}
