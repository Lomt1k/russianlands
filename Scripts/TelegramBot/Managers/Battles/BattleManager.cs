using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles
{
    public class BattleManager : GlobalManager
    {
        private List<Battle> _battles = new List<Battle>();
        private Dictionary<Player, Battle> _battlesByPlayers = new Dictionary<Player, Battle>();

        public override void OnManagerStart()
        {
        }

        public Battle StartBattle(Player opponentA, Player opponentB)
        {
            var battle = new BattlePVP(opponentA, opponentB);
            RegisterBattle(battle);
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
            RegisterBattle(battle);
            battle.StartBattle();
            return battle;
        }

        private void RegisterBattle(Battle battle)
        {
            _battles.Add(battle);
            if (battle.firstUnit is Player firstPlayer)
            {
                _battlesByPlayers[firstPlayer] = battle;
            }
            if (battle.secondUnit is Player secondPlayer)
            {
                _battlesByPlayers[secondPlayer] = battle;
            }
        }

        public Battle? GetCurrentBattle(Player player)
        {
            _battlesByPlayers.TryGetValue(player, out var battle);
            return battle;
        }

    }
}
