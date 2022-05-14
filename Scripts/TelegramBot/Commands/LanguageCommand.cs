using System;
using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Commands
{
    public class LanguageCommand : CommandBase
    {
        public override void Execute(GameSession session, string[] args)
        {
            if (args.Length != 1)
                return;

            if (!Enum.TryParse(args[0], out LanguageCode code))
                return;

            session.SetupLanguage(code);
            messageSender.SendTextMessage(session.chatId, $"Language changed to {code}");
        }
    }
}
