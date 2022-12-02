using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Managers;

namespace TextGameRPG.Scripts.Bot.Commands.Admin
{
    public class DrawCommand : CommandBase
    {
        public override bool isAdminCommand => true;

        public override async Task Execute(GameSession session, string[] args)
        {
            var battle = GlobalManagers.battleManager?.GetCurrentBattle(session.player);
            if (battle != null)
            {
                await battle.ForceBattleEndWithResult(session.player, Dialogs.Battle.BattleResult.Draw);
            }
        }
    }
}
