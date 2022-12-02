using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.Bot.Sessions;
using System;

namespace TextGameRPG.Scripts.Bot.Commands.Admin
{
    public class AddItemCommand : CommandBase
    {
        public override bool isAdminCommand => true;

        public override async Task Execute(GameSession session, string[] args)
        {
            if (args.Length < 1)
                return;

            InventoryItem? item = null;
            try
            {
                item = int.TryParse(args[0], out int itemId)
                ? new InventoryItem(itemId)
                : new InventoryItem(args[0].ToUpperInvariant());
            }
            catch (Exception ex)
            {
                await messageSender.SendTextMessage(session.chatId, $"Incorrect item ID\n\n{ex.Message}");
                return;
            }            

            bool success = session.player.inventory.TryAddItem(item);
            string message = success
                ? $"Added item with ID {item.id}:\n{item.GetFullName(session)}"
                : "Can`t add item: Inventory is full";
            await messageSender.SendTextMessage(session.chatId, message).ConfigureAwait(false);
        }

    }
}
