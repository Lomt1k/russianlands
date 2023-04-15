using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Cheats;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.Battles;

namespace TextGameRPG.Scripts.Bot.Commands.Cheats
{
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
}
