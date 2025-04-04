﻿using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Items.ItemAbilities;
using MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.Battles.Actions;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Units.ActionHandlers;

public static class ItemKeywordActionsHandler
{
    public static void HandleKeywords(BattleTurn battleTurn, InventoryItem? selectedItem,
        ref PlayerAttackAction generalAttack, ref List<IBattleAction> resultActionsList)
    {
        if (selectedItem == null)
            return;

        HandleGeneralAttackModifiers(battleTurn, selectedItem, ref generalAttack, ref resultActionsList);
        HandleOtherKeywords(battleTurn, selectedItem, ref generalAttack, ref resultActionsList);
    }

    public static void HandleGeneralAttackModifiers(BattleTurn battleTurn, InventoryItem selectedItem,
        ref PlayerAttackAction generalAttack, ref List<IBattleAction> resultActionsList)
    {
        var unit = battleTurn.unit;
        var statsByItems = (IStatsForUnitWithItems)unit.unitStats;
        var abilitiesDict = selectedItem.data.ablitityByType;
        // --- Абилки, которые приплюсовывают урон
        // Additional Fire Damage
        if (abilitiesDict.TryGetValue(AbilityType.AdditionalFireDamageKeyword, out var fireDamageAbility))
        {
            if (fireDamageAbility.TryChance())
            {
                var ability = (AdditionalFireDamageKeywordAbility)fireDamageAbility;
                generalAttack.damageInfo += new DamageInfo(fireDamage: ability.damageAmount);
                resultActionsList.Add(new AdditionalFireDamageAction());
            }
        }

        // Additional Cold Damage
        if (abilitiesDict.TryGetValue(AbilityType.AdditionalColdDamageKeyword, out var coldDamageAbility))
        {
            if (coldDamageAbility.TryChance())
            {
                var ability = (AdditionalColdDamageKeywordAbility)coldDamageAbility;
                generalAttack.damageInfo += new DamageInfo(coldDamage: ability.damageAmount);
                resultActionsList.Add(new AdditionalColdDamageAction());
            }
        }

        // Additional Lightning Damage
        if (abilitiesDict.TryGetValue(AbilityType.AdditionalLightningDamageKeyword, out var lightingDamageAbility))
        {
            if (lightingDamageAbility.TryChance())
            {
                var ability = (AdditionalLightningDamageKeywordAbility)lightingDamageAbility;
                generalAttack.damageInfo += new DamageInfo(lightningDamage: ability.damageAmount);
                resultActionsList.Add(new AdditionalLightingDamageAction());
            }
        }

        // --- Абилки, которые умножают урон
        // Last shot (bow only)
        var hasLastShot = abilitiesDict.TryGetValue(AbilityType.BowLastShotKeyword, out _);
        if (hasLastShot && unit.unitStats.currentArrows == 0)
        {
            battleTurn.enemy.unitStats.PredictDealDamageResult(generalAttack.damageInfo, out var predictedDamage, out _);
            generalAttack.damageInfo += predictedDamage;
            resultActionsList.Add(new BowLastShotAction());
        }

        // Rage
        if (abilitiesDict.TryGetValue(AbilityType.RageKeyword, out _))
        {
            statsByItems.rageAbilityCounter++;
            if (statsByItems.rageAbilityCounter == 3)
            {
                statsByItems.rageAbilityCounter = 0;
                battleTurn.enemy.unitStats.PredictDealDamageResult(generalAttack.damageInfo, out var predictedDamage, out _);
                generalAttack.damageInfo += predictedDamage;
                resultActionsList.Add(new RageAction());
            }
        }

        // Finishing (должна быть последней в этом методе!)
        if (abilitiesDict.TryGetValue(AbilityType.FinishingKeyword, out var finishingAbility))
        {
            var ability = (FinishingKeywordAbility)finishingAbility;
            var multiplicator = (float)ability.damageBonusPercentage / 100;
            battleTurn.enemy.unitStats.PredictDealDamageResult(generalAttack.damageInfo, out var predictedDamage, out var _);
            var bonusDamageIfFinishing = predictedDamage * multiplicator;
            battleTurn.enemy.unitStats.PredictDealDamageResult(generalAttack.damageInfo + bonusDamageIfFinishing, out _, out var resultHealth);

            if (resultHealth < 1)
            {
                generalAttack.damageInfo += bonusDamageIfFinishing;
                resultActionsList.Add(new FinishingAction(ability.damageBonusPercentage));
            }
        }
    }

    private static void HandleOtherKeywords(BattleTurn battleTurn, InventoryItem selectedItem,
        ref PlayerAttackAction generalAttack, ref List<IBattleAction> resultActionsList)
    {
        var abilitiesDict = selectedItem.data.ablitityByType;

        // Add Arrow
        if (abilitiesDict.TryGetValue(AbilityType.AddArrowKeyword, out var addArrowAbility))
        {
            if (addArrowAbility.TryChance())
            {
                resultActionsList.Add(new AddArrowAction());
            }
        }

        // Absorption
        if (abilitiesDict.TryGetValue(AbilityType.AbsorptionKeyword, out var absorptionAbility))
        {
            if (absorptionAbility.TryChance())
            {
                var enemyStats = battleTurn.enemy.unitStats;
                enemyStats.PredictDealDamageResult(generalAttack.damageInfo, out var resultDamage, out _);
                var healthToRestore = resultDamage.GetTotalValue();
                resultActionsList.Add(new AbsorptionAction(healthToRestore));
            }
        }

        // Mana Steal
        if (abilitiesDict.TryGetValue(AbilityType.StealManaKeyword, out var manaStealAbility))
        {
            if (manaStealAbility.TryChance())
            {
                resultActionsList.Add(new StealManaAction());
            }
        }

        // Add Mana
        if (abilitiesDict.TryGetValue(AbilityType.AddManaKeyword, out var addManaAbility))
        {
            if (addManaAbility.TryChance())
            {
                resultActionsList.Add(new AddManaKeywordAction());
            }
        }

        // Sanctions
        if (abilitiesDict.TryGetValue(AbilityType.SanctionsKeyword, out var sanctionsAbility))
        {
            if (sanctionsAbility.TryChance())
            {
                resultActionsList.Add(new SanctionsAction());
            }
        }

        // Stun
        if (abilitiesDict.TryGetValue(AbilityType.StunKeyword, out var stunAbility))
        {
            if (stunAbility.TryChance())
            {
                resultActionsList.Add(new StunAction());
            }
        }
    }

}
