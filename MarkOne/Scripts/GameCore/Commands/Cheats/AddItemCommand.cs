using System;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Commands.Cheats;

public class AddItemCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.Cheat;

    public override async Task Execute(GameSession session, string[] args)
    {
        if (args.Length < 1)
        {
            await SendManualMessage(session).FastAwait();
            return;
        }

        InventoryItem? item;
        try
        {
            item = int.TryParse(args[0], out var itemId)
            ? new InventoryItem(itemId)
            : new InventoryItem(args[0].ToUpperInvariant());
        }
        catch (Exception ex)
        {
            await messageSender.SendTextMessage(session.chatId, $"Incorrect item ID\n\n{ex.Message}").FastAwait();
            return;
        }

        session.player.inventory.ForceAddItem(item);
        var message = $"{item.GetView(session)}\n\n{Localization.Get(session, "dialog_inventory_item_added_state")}";
        await messageSender.SendTextMessage(session.chatId, message).FastAwait();
    }

    public async Task SendManualMessage(GameSession session)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Usage:".Bold());
        sb.AppendLine();
        sb.AppendLine($"/additem [itemId]");
        sb.AppendLine($"/additem [itemCode]");

        await messageSender.SendTextMessage(session.chatId, sb.ToString()).FastAwait();
    }

}
