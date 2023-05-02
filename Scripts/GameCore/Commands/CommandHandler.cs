using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Commands;

public static class CommandHandler
{
    private static readonly Dictionary<string, CommandBase> _commandsDictionary;

    static CommandHandler()
    {
        _commandsDictionary = new Dictionary<string, CommandBase>
        {
            // commands for all
            { "/start", new StartCommand() },

            // cheats
            { "//", new Cheats.CheatsCommand() },
            { "/cheats", new Cheats.CheatsCommand() },
            { "/fakeid", new FakeIdCommand() },
            { "/language", new LanguageCommand() },
            { "/addresource", new Cheats.AddResourceCommand() },
            { "/additem", new Cheats.AddItemCommand() },
            { "/battle", new Cheats.BattleCommand() },
            { "/win", new Cheats.WinCommand() },
            { "/lose", new Cheats.LoseCommand() },
            { "/draw", new Cheats.DrawCommand() },
            { "/status", new Admin.StatusCommand() },

            // admin commands
            // --- empty for now
        };
    }

    public static async Task HandleCommand(GameSession session, string unparsedCommand)
    {
        //Program.logger.Debug($"Command from @{session.actualUser.Username}: {unparsedCommand}");
        var elements = unparsedCommand.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var commandStr = elements[0];

        if (!_commandsDictionary.TryGetValue(commandStr, out var command))
            return;

        var access = command.commandGroup switch
        {
            CommandGroup.ForAll => true,
            CommandGroup.Admin => session.isAdmin,
            CommandGroup.Cheat => session.isAdmin || BotController.config.cheatsForAll || session.fakeChatId?.Identifier != null,
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
