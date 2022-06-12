using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                { "/language", new LanguageCommand() },
                { "/additem", new AddItemCommand() },
                { "/test", new TestCommand() },
            };
        }

        public static async Task HandleCommand(GameSession session, string unparsedCommand)
        {
            //Program.logger.Debug($"Command from @{session.actualUser.Username}: {unparsedCommand}");
            var elements = unparsedCommand.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string commandStr = elements[0];

            if (!_commandsDictionary.TryGetValue(commandStr, out var command))
                return;

            if (elements.Length > 1)
            {
                await command.Execute(session, elements.Skip(1).ToArray());
                return;
            }
            else
            {
                await command.Execute(session, new string[] { });
            }
        }
    }
}
