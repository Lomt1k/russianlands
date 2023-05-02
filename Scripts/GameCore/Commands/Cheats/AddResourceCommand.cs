using System;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Commands.Cheats;

internal class AddResourceCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.Cheat;

    public override async Task Execute(GameSession session, string[] args)
    {
        if (args.Length != 2
            || !Enum.TryParse(args[0], ignoreCase: true, out ResourceId resourceId)
            || !int.TryParse(args[1], out var amount))
        {
            await SendManualMessage(session);
            return;
        }

        var resourceData = new ResourceData(resourceId, amount);
        session.player.resources.ForceAdd(resourceData);

        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "battle_result_header_rewards"));
        sb.AppendLine(resourceData.GetLocalizedView(session));

        await messageSender.SendTextMessage(session.chatId, sb.ToString()).FastAwait();
    }

    public async Task SendManualMessage(GameSession session)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Usage:".Bold());
        sb.AppendLine();
        foreach (ResourceId resourceId in Enum.GetValues(typeof(ResourceId)))
        {
            sb.AppendLine($"/addresource {resourceId} [amount]");
        }
        await messageSender.SendTextMessage(session.chatId, sb.ToString()).FastAwait();
    }
}
