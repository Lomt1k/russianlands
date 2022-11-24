﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Commands.Admin;
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
                { "/start", new StartCommand() },
                { "/language", new LanguageCommand() },

                //admin commands
                { "/status", new StatusCommand() },
                { "/additem", new AddItemCommand() },
                { "/test", new TestCommand() },
                { "/battle", new BattleCommand() },
                { "/win", new WinCommand() },
                { "/lose", new LoseCommand() },
                { "/draw", new DrawCommand() },
            };
        }

        public static async Task HandleCommand(GameSession session, string unparsedCommand)
        {
            //Program.logger.Debug($"Command from @{session.actualUser.Username}: {unparsedCommand}");
            var elements = unparsedCommand.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string commandStr = elements[0];

            if (!_commandsDictionary.TryGetValue(commandStr, out var command))
                return;

            if (command.isAdminCommand && !session.isAdmin)
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
