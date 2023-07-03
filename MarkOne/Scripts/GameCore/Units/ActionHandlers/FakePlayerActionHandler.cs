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
        var (battleActions, selectedItem) = FakePlayerActionSelector.SelectAction(battleTurn);

        var random = new Random();
        var delay = random.Next(3000, 5000);
        // Дополнительная задержка в начале битвы, когда настоящий игрок ходит вторым
        if (battleTurn.isFirstUnit && battleTurn.turnNumber == 1)
        {
            delay += random.Next(2000, 5000);
        }
        // Симуляция АФК на старте битвы
        if (battleTurn.turnNumber == 1 && Randomizer.TryPercentage(15))
        {
            var quotient = random.NextSingle();
            delay += 5_000 + (int)(10_000 * quotient);
        }
        // Дополнительная задержка при выборе свитка
        if (selectedItem.data.itemType == ItemType.Scroll)
        {
            delay += random.Next(1500, 2500);
        }
        // Рандомная задержка начиная с 4-го хода (на 3-м ходу игрок использовал посох, дальше он типа тратит больше времени на обдумывание - как походить)
        if (battleTurn.turnNumber >=  4 && Randomizer.TryPercentage(25))
        {
            delay += random.Next(3000, 6000);
        }

        await Task.Delay(delay).FastAwait();
        return battleActions;
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
