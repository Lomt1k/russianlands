using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Commands.Cheats
{
    internal class AddResourceCommand : CommandBase
    {
        public override CommandGroup commandGroup => CommandGroup.Cheat;

        public override async Task Execute(GameSession session, string[] args)
        {
            if (args.Length != 2
                || !Enum.TryParse(args[0], ignoreCase: true, out ResourceId resourceId)
                || !int.TryParse(args[1], out int amount))
            {
                await SendManualMessage(session);
                return;
            }

            session.player.resources.ForceAdd(resourceId, amount);

            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "battle_result_header_rewards"));
            sb.AppendLine(resourceId.GetLocalizedView(session, amount));

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
}
