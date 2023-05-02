using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Dialogs.Cheats;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Battles;

namespace MarkOne.Scripts.Bot.Commands.Cheats;

public class CheatsCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.Cheat;

    private static readonly BattleManager battleManager = Services.Get<BattleManager>();

    public override async Task Execute(GameSession session, string[] args)
    {
        if (battleManager.GetCurrentBattle(session.player) != null)
        {
            await new BattleCheatsDialog(session).Start().FastAwait();
        }
        else
        {
            await new CheatsDialog(session).Start().FastAwait();
        }
    }
}
