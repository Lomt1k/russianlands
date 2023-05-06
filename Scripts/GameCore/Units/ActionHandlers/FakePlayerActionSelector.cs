using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.Battles.Actions;
using MarkOne.Scripts.GameCore.Units.Stats;
using System.Collections.Generic;
using System.Linq;

namespace MarkOne.Scripts.GameCore.Units.ActionHandlers;
public struct PredictedAttackData
{
    public InventoryItem item { get; init; }
    public List<IBattleAction> battleActions { get; init; }
    public PlayerAttackAction generalAttack { get; init; }
    public bool isAvailable { get; init; }
    public bool isLethalDamage { get; init; }
    public int predictedDamage { get ; init; }
}

public static class FakePlayerActionSelector
{
    public static (List<IBattleAction> battleActions, InventoryItem selectedItem) SelectAction(BattleTurn battleTurn)
    {
        var fakePlayer = (FakePlayer)battleTurn.unit;
        var enemy = battleTurn.enemy;

        var allAttacks = PredictAllAttacks(battleTurn, fakePlayer, enemy);
        var item = SelectItemForAttack(allAttacks);

        // apply selection
        fakePlayer.unitStats.OnUseItemInBattle(item);
        var generalAttackAction = new PlayerAttackAction(item);
        var result = new List<IBattleAction> { generalAttackAction };
        ItemKeywordActionsHandler.HandleKeywords(battleTurn, item, ref generalAttackAction, ref result);
        return (result, item);
    }

    private static List<PredictedAttackData> PredictAllAttacks(BattleTurn battleTurn, FakePlayer fakePlayer, IBattleUnit enemy)
    {
        var fakePlayerStats = (FakePlayerStats)fakePlayer.unitStats;
        var rageForRestore = fakePlayerStats.rageAbilityCounter;
        var result = new List<PredictedAttackData>();
        foreach (var item in GetAllAttackItems(fakePlayer))
        {
            var predictedAttack = PredictAttack(battleTurn, fakePlayer, enemy, item);
            result.Add(predictedAttack);
            fakePlayerStats.rageAbilityCounter = rageForRestore;
        }
        return result;
    }

    private static IEnumerable<InventoryItem> GetAllAttackItems(FakePlayer fakePlayer)
    {
        var equipped = fakePlayer.equippedItems;
        var sword = equipped[ItemType.Sword];
        if (sword != null)
        {
            yield return sword;
        }
        var bow = equipped[ItemType.Bow];
        if (bow != null && fakePlayer.unitStats.currentArrows > 0)
        {
            yield return bow;
        }
        var stick = equipped[ItemType.Stick];
        if (stick != null && fakePlayer.unitStats.currentStickCharge >= InventoryItem.requiredStickCharge)
        {
            yield return stick;
        }
        for (var i = 0; i < ItemType.Scroll.GetSlotsCount(); i++)
        {
            var scroll = equipped[ItemType.Scroll, i];
            if (scroll != null)
            {         
                yield return scroll;
            }
        }
    }

    private static PredictedAttackData PredictAttack(BattleTurn battleTurn, FakePlayer fakePlayer, IBattleUnit enemy, InventoryItem item)
    {
        var battleActions = new List<IBattleAction>();
        var generalAttackAction = new PlayerAttackAction(item);
        battleActions.Add(generalAttackAction);
        ItemKeywordActionsHandler.HandleKeywords(battleTurn, item, ref generalAttackAction, ref battleActions);

        var isAvailable = item.data.itemType switch
        {
            ItemType.Bow => fakePlayer.unitStats.currentArrows > 0,
            ItemType.Stick => fakePlayer.unitStats.currentStickCharge >= InventoryItem.requiredStickCharge,
            ItemType.Scroll => fakePlayer.unitStats.currentMana >= item.manaCost,
            _ => true
        };
        enemy.unitStats.PredictDealDamageResult(generalAttackAction.damageInfo, out var predictedDamage, out int _);

        var isLethalDamage = predictedDamage.GetTotalValue() >= enemy.unitStats.currentHP;

        return new PredictedAttackData
        {
            item = item,
            battleActions = battleActions,
            generalAttack = generalAttackAction,
            isAvailable = isAvailable,
            isLethalDamage = isLethalDamage,
            predictedDamage = predictedDamage.GetTotalValue(),
        };
    }

    private static InventoryItem SelectItemForAttack(List<PredictedAttackData> predictedAttacks)
    {
        var hasAvailableLethalDamage = predictedAttacks.Any(x => x.isLethalDamage && x.isAvailable);
        if (hasAvailableLethalDamage)
        {
            return predictedAttacks.Where(x => x.isLethalDamage && x.isAvailable).OrderByDescending(x => x.predictedDamage).First().item;
        }

        var bestAttack = predictedAttacks.OrderByDescending(x => x.predictedDamage).First();
        var bestAvailableAttack = predictedAttacks.Where(x => x.isAvailable).OrderByDescending(x => x.predictedDamage).First();
        var hasAvailableStick = predictedAttacks.Any(x => x.item.data.itemType == ItemType.Stick && x.isAvailable);

        if (bestAttack.item == bestAvailableAttack.item)
        {
            if (hasAvailableStick && bestAttack.item.data.itemType != ItemType.Stick)
            {
                var stickAttack = predictedAttacks.Where(x => x.item.data.itemType == ItemType.Stick).First();
                var damageRate = (float)stickAttack.predictedDamage / bestAttack.predictedDamage;
                return damageRate > 0.5 ? stickAttack.item : bestAttack.item;
            }
            return bestAttack.item;
        }

        if (hasAvailableStick)
        {
            return predictedAttacks.Where(x => x.item.data.itemType == ItemType.Stick).First().item;
        }

        var hasAvailableBow = predictedAttacks.Any(x => x.item.data.itemType == ItemType.Bow && x.isAvailable);
        if (hasAvailableBow)
        {
            return predictedAttacks.Where(x => x.item.data.itemType == ItemType.Bow).First().item;
        }

        return bestAvailableAttack.item;
    }


}
