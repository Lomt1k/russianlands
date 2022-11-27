using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords;
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.GameCore.Managers.Battles.Actions;

namespace TextGameRPG.Scripts.GameCore.Units.ActionHandlers
{
    public static class ItemKeywordActionsHandler
    {
        public static void HandleKeywords(BattleTurn battleTurn, InventoryItem? selectedItem,
            ref PlayerAttackAction generalAttack, ref List<IBattleAction> resultActionsList)
        {
            if (selectedItem == null)
                return;

            var unit = battleTurn.unit;
            var abilitiesDict = selectedItem.data.ablitityByType;
            HandleGeneralAttackModifiers(battleTurn, selectedItem, ref generalAttack, ref resultActionsList);

            // Add Arrow
            if (abilitiesDict.TryGetValue(AbilityType.AddArrowKeyword, out var addArrowAbility))
            {
                if (addArrowAbility.TryChance())
                {
                    resultActionsList.Add(new AddArrowAction());
                }
            }

            // Mana Steal
            if (abilitiesDict.TryGetValue(AbilityType.StealManaKeyword, out var manaStealAbility))
            {
                if (manaStealAbility.TryChance())
                {
                    resultActionsList.Add(new StealManaAction(battleTurn));
                }
            }

        }

        public static void HandleGeneralAttackModifiers(BattleTurn battleTurn, InventoryItem selectedItem,
            ref PlayerAttackAction generalAttack, ref List<IBattleAction> resultActionsList)
        {
            var unit = battleTurn.unit;
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
            bool hasLastShot = abilitiesDict.TryGetValue(AbilityType.BowLastShotKeyword, out var lastShotAbility);
            if (hasLastShot && unit.unitStats.currentArrows == 0)
            {
                generalAttack.damageInfo *= 2;
                resultActionsList.Add(new BowLastShotAction());
            }
        }

    }
}
