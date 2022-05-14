using System;
using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Commands
{
    public static class CommandHandler
    {
        private static Dictionary<string, CommandBase> _commandsDictionary;

        static CommandHandler()
        {
            _commandsDictionary = new Dictionary<string, CommandBase>
            {
                { "/language", new LanguageCommand() }
            };
        }

        public static void HandleCommand(GameSession session, string unparsedCommand)
        {
            //Program.logger.Debug($"Command from @{session.actualUser.Username}: {unparsedCommand}");
            var elements = unparsedCommand.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string commandStr = elements[0];

            if (!_commandsDictionary.TryGetValue(commandStr, out var command))
                return;

            if (elements.Length > 1)
            {
                command.Execute(session, elements.Skip(1).ToArray());
                return;
            }
            else
            {
                command.Execute(session, new string[] { });
            }
        }
    }
}
