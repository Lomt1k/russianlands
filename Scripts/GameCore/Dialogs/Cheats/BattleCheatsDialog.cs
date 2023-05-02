using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Commands;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs.Cheats;

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
