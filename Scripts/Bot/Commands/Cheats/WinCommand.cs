using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Managers;

namespace TextGameRPG.Scripts.Bot.Commands.Cheats
{
    public class WinCommand : CommandBase
    {
        public override CommandGroup commandGroup => CommandGroup.Cheat;

        public override async Task Execute(GameSession session, string[] args)
        {
            var battle = GlobalManagers.battleManager?.GetCurrentBattle(session.player);
            if (battle != null)
            {
                await battle.ForceBattleEndWithResult(session.player, Dialogs.Battle.BattleResult.Win);
            }            
        }
    }
}
