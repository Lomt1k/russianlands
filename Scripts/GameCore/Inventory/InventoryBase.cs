using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Items;
using System.Text.Json.Serialization;
using JsonKnownTypes;

namespace TextGameRPG.Scripts.GameCore.Inventory
{
    [JsonConverter(typeof(JsonKnownTypesConverter<InventoryBase>))]
    public class InventoryBase
    {
        public List<ItemBase> items { get; private set; } = new List<ItemBase>();

        public InventoryBase()
        {
        }

        public InventoryBase(IEnumerable<ItemBase> items) : this()
        {
            this.items = items.ToList();
        }
    }
}
