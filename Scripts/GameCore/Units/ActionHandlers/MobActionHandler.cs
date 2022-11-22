using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.GameCore.Managers.Battles.Actions;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.GameCore.Units.ActionHandlers
{
    public class MobActionHandler : IBattleActionHandler
    {
        private Mob mob { get; }

        public MobActionHandler(Mob mob)
        {
            this.mob = mob;
        }

        public async Task<IBattleAction?> GetAttackAction(BattleTurn battleTurn)
        {
            await Task.Delay(BattleTurn.MOB_TURN_MILISECONDS_DELAY);

            var availableAttacks = GetAvailableAttacks();
            if (availableAttacks.Count == 0)
            {
                return null;
            }

            var attackIndex = new Random().Next(availableAttacks.Count);
            var attackAction = new MobAttackAction(availableAttacks[attackIndex], mob.gradeMult);
            return attackAction;
        }

        private List<MobAttack> GetAvailableAttacks()
        {
            var result = new List<MobAttack>();
            foreach (var attack in mob.mobData.mobAttacks)
            {
                if (attack.manaCost <= mob.unitStats.currentMana)
                {
                    result.Add(attack);
                }
            }
            return result;
        }

        public bool TryAddShieldOnStartEnemyTurn(out DamageInfo damageInfo)
        {
            //ignored
            damageInfo = DamageInfo.Zero;
            return false;
        }

        public List<IBattleAction> GetEveryTurnActions(BattleTurn battleTurn)
        {
            //ignored
            return new List<IBattleAction>();
        }

    }
}
