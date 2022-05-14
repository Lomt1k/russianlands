using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Items;
using System.Text.Json.Serialization;
using JsonKnownTypes;

namespace TextGameRPG.Scripts.GameCore.Inventory
{
    [JsonConverter(typeof(JsonKnownTypesConverter<Inventory>))]
    public class Inventory
    {
        public List<InventoryItem> items { get; private set; } = new List<InventoryItem>();

        public Inventory()
        {
        }

        public Inventory(IEnumerable<InventoryItem> items) : this()
        {
            this.items = items.ToList();
        }
    }
}
