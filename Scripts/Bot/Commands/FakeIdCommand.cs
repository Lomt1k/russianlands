using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Commands
{
    internal class FakeIdCommand : CommandBase
    {
        public override CommandGroup commandGroup => CommandGroup.ForAll;

        public override async Task Execute(GameSession session, string[] args)
        {
            var isCheatsForAll = TelegramBot.instance.config.cheatsForAll;
            if (args.Length != 1 || !long.TryParse(args[0], out long fakeId))
            {
                if (isCheatsForAll || session.isAdmin)
                {
                    await SendManualMessage(session);
                }
                return;
            }

            if (!isCheatsForAll && !session.isAdmin && fakeId != 0)
                return;

            var sessionManager = TelegramBot.instance.sessionManager;
            var realId = session.actualUser.Id;

            await sessionManager.CloseSession(fakeId)
                .ConfigureAwait(false);
            await sessionManager.CloseSession(realId)
                .ConfigureAwait(false);

            sessionManager.Cheat_SetFakeId(realId, fakeId);
            var message = fakeId == 0
                ? $"Game will be restarted with your real account\nTelegram Id: {realId}"
                : $"Game will be restarted with\nTelegram Id: {fakeId}\n\n{Emojis.ElementWarning} To restore your real account use:\n/fakeid 0";

            await messageSender.SendTextDialog(realId, message, "Restart")
                .ConfigureAwait(false);
        }

        public async Task SendManualMessage(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage:".Bold());
            sb.AppendLine();
            sb.AppendLine($"/fakeid [telegram_id]");
            await messageSender.SendTextMessage(session.chatId, sb.ToString())
                    .ConfigureAwait(false);
        }

    }
}
