﻿using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Commands;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.Bot.Dialogs.Cheats;

public class BattleCheatsDialog : DialogBase
{
    public BattleCheatsDialog(GameSession _session) : base(_session)
    {
        RegisterButton("Win", BattleWinCommand);
        RegisterButton("Lose", BattleLoseCommand);
        RegisterButton("Draw", BattleDrawCommand);
    }

    public override async Task Start()
    {
        var header = "Battle Cheats".Bold();
        await SendDialogMessage(header, GetKeyboardWithRowSizes(3)).FastAwait();
    }

    public async Task BattleWinCommand()
    {
        await CommandHandler.HandleCommand(session, "/win").FastAwait();
    }

    public async Task BattleLoseCommand()
    {
        await CommandHandler.HandleCommand(session, "/lose").FastAwait();
    }

    public async Task BattleDrawCommand()
    {
        await CommandHandler.HandleCommand(session, "/draw").FastAwait();
    }

}
