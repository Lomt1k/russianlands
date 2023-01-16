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
                || !Enum.TryParse(args[0], ignoreCase: true, out ResourceType resourceType)
                || !int.TryParse(args[1], out int amount))
            {
                await SendManualMessage(session);
                return;
            }

            session.player.resources.ForceAdd(resourceType, amount);

            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "battle_result_header_rewards"));
            sb.AppendLine(resourceType.GetLocalizedView(session, amount));

            await messageSender.SendTextMessage(session.chatId, sb.ToString())
                .ConfigureAwait(false);
        }

        public async Task SendManualMessage(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage:".Bold());
            sb.AppendLine();
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                sb.AppendLine($"/addresource {resourceType} [amount]");
            }
            await messageSender.SendTextMessage(session.chatId, sb.ToString())
                    .ConfigureAwait(false);
        }
    }
}
