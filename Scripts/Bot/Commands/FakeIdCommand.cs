using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Commands
{
    internal class FakeIdCommand : CommandBase
    {
        public override CommandGroup commandGroup => CommandGroup.Cheat;

        public override async Task Execute(GameSession session, string[] args)
        {
            if (args.Length != 1 || !long.TryParse(args[0], out long fakeId))
            {
                await SendManualMessage(session);
                return;
            }

            var sessionManager = TelegramBot.instance.sessionManager;
            var realId = session.actualUser.Id;

            await sessionManager.CloseSession(fakeId).FastAwait();
            await sessionManager.CloseSession(realId).FastAwait();

            sessionManager.Cheat_SetFakeId(realId, fakeId);
            var message = fakeId == 0
                ? $"Game will be restarted with your real account\n<b>Telegram Id:</b> {realId}"
                : $"Game will be restarted with\n<b>Telegram Id:</b> {fakeId}\n\n{Emojis.ElementWarning} To restore your real account use:\n/fakeid 0";

            await messageSender.SendTextDialog(realId, message, "Restart").FastAwait();
        }

        public async Task SendManualMessage(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage:".Bold());
            sb.AppendLine();
            sb.AppendLine($"/fakeid [telegram_id]");
            await messageSender.SendTextMessage(session.chatId, sb.ToString()).FastAwait();
        }

    }
}
