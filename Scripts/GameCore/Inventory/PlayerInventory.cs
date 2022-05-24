using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Items;
using System.Text.Json.Serialization;
using JsonKnownTypes;

namespace TextGameRPG.Scripts.GameCore.Inventory
{
    [JsonConverter(typeof(JsonKnownTypesConverter<PlayerInventory>))]
    public class PlayerInventory
    {
        public List<InventoryItem> items { get; private set; } = new List<InventoryItem>();

        public PlayerInventory()
        {
        }

        public PlayerInventory(IEnumerable<InventoryItem> items) : this()
        {
            this.items = items.ToList();
        }
    }
}
