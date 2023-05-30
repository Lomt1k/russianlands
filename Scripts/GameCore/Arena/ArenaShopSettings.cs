using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Shop;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MarkOne.Scripts.GameCore.Arena;
[JsonObject]
public class ArenaShopSettings : IGameDataWithId<byte>
{
    public byte id { get; set; }
    public List<ShopItemBase> mainCategoryItems { get; set; } = new();
    public List<ShopItemBase> exchangeCategoryItems { get; set; } = new();
    public List<ArenaShopItemPattern> tempItemPatterns { get; set; } = new();
    public int freeChipsCount { get; set; }
    public int freeChipsDelayInSeconds { get; set; }
    public int tempItemsRefreshDelayInSeconds { get; set; }
    public int tempItemsRefreshPriceInDiamonds { get; set; }
    [JsonIgnore]
    public ResourceData freeChipsResourceData { get; private set; }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        freeChipsResourceData = new ResourceData(ResourceId.ArenaChip, freeChipsCount);
    }

    public void OnSetupAppMode(AppMode appMode)
    {
        // ignored
    }
}
