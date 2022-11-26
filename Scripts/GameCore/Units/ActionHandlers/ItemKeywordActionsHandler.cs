using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.GameCore.Managers.Battles.Actions;

namespace TextGameRPG.Scripts.GameCore.Units.ActionHandlers
{
    public static class ItemKeywordActionsHandler
    {
        public static void AddKeywords(BattleTurn battleTurn, InventoryItem? selectedItem,
            ref PlayerAttackAction generalAttack, ref List<IBattleAction> resultActionsList)
        {
            if (selectedItem == null)
                return;

            //TODO
        }

    }
}
