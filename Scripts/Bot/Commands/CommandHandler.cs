using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Commands
{
    public static class CommandHandler
    {
        private static Dictionary<string, CommandBase> _commandsDictionary;

        static CommandHandler()
        {
            _commandsDictionary = new Dictionary<string, CommandBase>
            {
                // commands for all
                { "/start", new StartCommand() },

                // cheats
                { "//", new Cheats.CheatsCommand() },
                { "/cheats", new Cheats.CheatsCommand() },
                { "/language", new Cheats.LanguageCommand() },
                { "/addresource", new Cheats.AddResourceCommand() },
                { "/additem", new Cheats.AddItemCommand() },
                { "/test", new Cheats.TestCommand() },
                { "/battle", new Cheats.BattleCommand() },
                { "/win", new Cheats.WinCommand() },
                { "/lose", new Cheats.LoseCommand() },
                { "/draw", new Cheats.DrawCommand() },

                // admin commands
                { "/status", new Admin.StatusCommand() },
            };
        }

        public static async Task HandleCommand(GameSession session, string unparsedCommand)
        {
            //Program.logger.Debug($"Command from @{session.actualUser.Username}: {unparsedCommand}");
            var elements = unparsedCommand.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string commandStr = elements[0];

            if (!_commandsDictionary.TryGetValue(commandStr, out var command))
                return;

            bool access = command.commandGroup switch
            {
                CommandGroup.ForAll => true,
                CommandGroup.Admin => session.isAdmin,
                CommandGroup.Cheat => session.isAdmin || TelegramBot.instance.config.cheatsForAll,
                _ => false
            };
            if (!access)
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
