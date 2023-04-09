using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Managers;
using TextGameRPG.Scripts.GameCore.Managers.Battles;

namespace TextGameRPG.Scripts.Bot.Commands.Cheats
{
    public class DrawCommand : CommandBase
    {
        public override CommandGroup commandGroup => CommandGroup.Cheat;

        private static readonly BattleManager battleManager = Singletones.Get<BattleManager>();

        public override async Task Execute(GameSession session, string[] args)
        {
            var battle = battleManager.GetCurrentBattle(session.player);
            if (battle != null)
            {
                await battle.ForceBattleEndWithResult(session.player, Dialogs.Battle.BattleResult.Draw);
            }
        }
    }
}
