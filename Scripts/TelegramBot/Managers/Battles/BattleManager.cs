using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public class BattleManager : GlobalManager
    {
        public List<Battle> battles = new List<Battle>();

        public override void OnManagerStart()
        {
        }

        public async Task<Battle> StartBattle(Player opponentA, Player opponentB)
        {
            var battle = new BattlePVP(opponentA, opponentB);
            battles.Add(battle);
            await battle.StartBattleAsync();
            return battle;
        }

        public async Task<Battle> StartBattle(Player player, MobData mobData)
        {
            var mob = new Mob(player.session, mobData);
            var battle = new BattlePVE(player, mob);
            battles.Add(battle);
            await battle.StartBattleAsync();
            return battle;
        }

    }
}
