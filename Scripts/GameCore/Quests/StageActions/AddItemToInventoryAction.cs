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
            var item = new InventoryItem(itemId);
            var success = inventory.TryAddItem(item);
            if (!success || !forceEquip)
                return Task.CompletedTask;

            if (item.data.itemType.IsMultiSlot())
            {
                inventory.EquipMultiSlot(item, slotIdForMultiSlot);
            }
            else
            {
                inventory.EquipSingleSlot(item);
            }
            return Task.CompletedTask;
        }
    }
}
