using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Items.ItemAbilities;
using MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.Battles.Actions;
using MarkOne.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Units.ActionHandlers;
/*
 * Дублирует PlayerActionHandler за исключением GetActionsBySelectedItem
 */
public class FakePlayerActionHandler : IBattleActionHandler
{
    public FakePlayer fakePlayer { get; }

    public FakePlayerActionHandler(FakePlayer _fakePlayer)
    {
        fakePlayer = _fakePlayer;
    }

    public async Task<List<IBattleAction>> GetActionsBySelectedItem(BattleTurn battleTurn)
    {
        throw new NotImplementedException();
    }

    public bool TryAddShieldOnEnemyTurn(out DamageInfo damageInfo)
    {
        damageInfo = DamageInfo.Zero;

        var shield = fakePlayer.equippedItems[ItemType.Shield];
        if (shield == null)
            return false;

        if (!shield.data.ablitityByType.TryGetValue(AbilityType.BlockIncomingDamageEveryTurn, out var shieldBlockAbility))
            return false;

        var blockAbility = (BlockIncomingDamageEveryTurnAbility)shieldBlockAbility;
        var success = Randomizer.TryPercentage(blockAbility.chanceToSuccessPercentage);
        if (!success)
            return false;

        damageInfo = new DamageInfo(
            physicalDamage: blockAbility.physicalDamage,
            fireDamage: blockAbility.fireDamage,
            coldDamage: blockAbility.coldDamage,
            lightningDamage: blockAbility.lightningDamage);
        return true;
    }

    public bool TryAddSwordBlockOnEnemyTurn(out DamageInfo damageInfo)
    {
        damageInfo = DamageInfo.Zero;

        var sword = fakePlayer.equippedItems[ItemType.Sword];
        if (sword == null)
            return false;

        if (!sword.data.ablitityByType.TryGetValue(AbilityType.SwordBlockEveryTurnKeyword, out var swordBlockAbility))
            return false;

        var blockAbility = (SwordBlockKeywordAbility)swordBlockAbility;
        var success = Randomizer.TryPercentage(blockAbility.chanceToSuccessPercentage);
        if (!success)
            return false;

        damageInfo = new DamageInfo(
            physicalDamage: blockAbility.physicalDamage,
            fireDamage: blockAbility.fireDamage,
            coldDamage: blockAbility.coldDamage,
            lightningDamage: blockAbility.lightningDamage);
        return true;
    }

    public IEnumerable<IBattleAction> GetEveryTurnActions(BattleTurn battleTurn)
    {
        var allEquipped = fakePlayer.equippedItems.allEquipped;

        // --- Restore Health
        var restoreHealthAction = new RestoreHealthAction();
        foreach (var item in allEquipped)
        {
            if (item.data.ablitityByType.TryGetValue(AbilityType.RestoreHealthEveryTurn, out var ability))
            {
                var restoreHealthAbility = (RestoreHealthEveryTurnAbility)ability;
                if (restoreHealthAbility.TryChance())
                {
                    restoreHealthAction.Add(item, restoreHealthAbility.healthValue);
                }
            }
        }
        if (restoreHealthAction.healthAmount > 0)
        {
            yield return restoreHealthAction;
        }

        // --- Add Mana
        var addManaAction = new AddManaAction();
        foreach (var item in allEquipped)
        {
            if (item.data.ablitityByType.TryGetValue(AbilityType.AddManaEveryTurn, out var ability))
            {
                var addManaAbility = (AddManaEveryTurnAbility)ability;
                if (addManaAbility.TryChance())
                {
                    addManaAction.Add(item, addManaAbility.manaValue);
                }
            }
        }
        if (addManaAction.manaAmount > 0)
        {
            yield return addManaAction;
        }

        // ...
    }

}
