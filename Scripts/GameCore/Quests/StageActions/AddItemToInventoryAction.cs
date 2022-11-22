using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions
{
    [JsonObject]
    public class AddItemToInventoryAction : StageActionBase
    {
        [JsonProperty]
        public int itemId;
        [JsonProperty]
        public bool forceEquip;
        [JsonProperty]
        public int slotIdForMultiSlot;

        public override Task Execute(GameSession session)
        {
            var inventory = session.player.inventory;
            var addedItem = inventory.TryAddItem(itemId);
            if (addedItem == null || !forceEquip)
                return Task.CompletedTask;

            if (addedItem.data.itemType.IsMultiSlot())
            {
                inventory.EquipMultiSlot(addedItem, slotIdForMultiSlot);
            }
            else
            {
                inventory.EquipSingleSlot(addedItem);
            }
            return Task.CompletedTask;
        }
    }
}
