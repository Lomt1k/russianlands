using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Battles;

namespace MarkOne.Scripts.Bot.Commands.Cheats;

public class DrawCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.Cheat;

    private static readonly BattleManager battleManager = Services.Get<BattleManager>();

    public override async Task Execute(GameSession session, string[] args)
    {
        var battle = battleManager.GetCurrentBattle(session.player);
        if (battle != null)
        {
            await battle.ForceBattleEndWithResult(session.player, Dialogs.Battle.BattleResult.Draw);
        }
    }
}
