using System.Collections.Generic;
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

        public Battle StartBattle(Player opponentA, Player opponentB)
        {
            var battle = new BattlePVP(opponentA, opponentB);
            battles.Add(battle);
            battle.StartBattle();
            return battle;
        }

        public Battle StartBattle(Player player, MobData mobData)
        {
            var mob = new Mob(player.session, mobData);
            return StartBattle(player, mob);
        }

        public Battle StartBattle(Player player, Mob mob)
        {
            var battle = new BattlePVE(player, mob);
            battles.Add(battle);
            battle.StartBattle();
            return battle;
        }

    }
}
