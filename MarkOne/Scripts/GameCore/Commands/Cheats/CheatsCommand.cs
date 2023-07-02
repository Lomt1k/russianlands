using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Dialogs.Cheats;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Commands.Cheats;

public class CheatsCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.Cheat;

    private static readonly BattleManager battleManager = Services.ServiceLocator.Get<BattleManager>();

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
