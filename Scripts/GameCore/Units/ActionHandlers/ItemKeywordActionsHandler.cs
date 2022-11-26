using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
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

            // --- Last shot (bow only)
            bool hasLastShot = abilitiesDict.TryGetValue(AbilityType.BowLastShotKeyword, out var lastShotAbility);
            if (hasLastShot && unit.unitStats.currentArrows == 0)
            {
                generalAttack.damageInfo *= 2;
                resultActionsList.Add(new BowLastShotAction());
            }

        }

    }
}
