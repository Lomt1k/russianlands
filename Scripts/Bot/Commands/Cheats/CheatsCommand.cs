using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Cheats;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Managers;

namespace TextGameRPG.Scripts.Bot.Commands.Cheats
{
    public class CheatsCommand : CommandBase
    {
        public override CommandGroup commandGroup => CommandGroup.Cheat;

        public override async Task Execute(GameSession session, string[] args)
        {
            if (GlobalManagers.battleManager?.GetCurrentBattle(session.player) != null)
            {
                await new BattleCheatsDialog(session).Start().FastAwait();
            }
            else
            {
                await new CheatsDialog(session).Start().FastAwait();
            }
        }
    }
}
