using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Services.Battles;
using TextGameRPG.Scripts.GameCore.Services.Battles.Actions;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.GameCore.Units.ActionHandlers;

public class MobActionHandler : IBattleActionHandler
{
    private Mob mob { get; }

    public MobActionHandler(Mob mob)
    {
        this.mob = mob;
    }

    public async Task<List<IBattleAction>> GetActionsBySelectedItem(BattleTurn battleTurn)
    {
        await Task.Delay(BattleTurn.MOB_TURN_MILISECONDS_DELAY);
        var result = new List<IBattleAction>();

        var availableAttacks = GetAvailableAttacks();
        if (availableAttacks.Count == 0)
        {
            return result;
        }

        var attackIndex = new Random().Next(availableAttacks.Count);
        var attackAction = new MobAttackAction(availableAttacks[attackIndex]);
        result.Add(attackAction);
        return result;
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

    public bool TryAddShieldOnEnemyTurn(out DamageInfo damageInfo)
    {
        //ignored
        damageInfo = DamageInfo.Zero;
        return false;
    }

    public bool TryAddSwordBlockOnEnemyTurn(out DamageInfo damageInfo)
    {
        //ignored
        damageInfo = DamageInfo.Zero;
        return false;
    }

    public IEnumerable<IBattleAction> GetEveryTurnActions(BattleTurn battleTurn)
    {
        //ignored
        return new IBattleAction[0];
    }

}
