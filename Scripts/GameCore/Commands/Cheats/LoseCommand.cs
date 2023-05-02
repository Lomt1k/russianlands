using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Commands.Cheats;

public class LoseCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.Cheat;

    private static readonly BattleManager battleManager = ServiceLocator.Get<BattleManager>();

    public override async Task Execute(GameSession session, string[] args)
    {
        var battle = battleManager.GetCurrentBattle(session.player);
        if (battle != null)
        {
            await battle.ForceBattleEndWithResult(session.player, BattleResult.Lose);
        }
    }
}
