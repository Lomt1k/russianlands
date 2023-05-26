using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Shop;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Arena;
[JsonObject]
public class ArenaShopSettings : IGameDataWithId<byte>
{
    public byte id { get; set; }
    public List<ShopItemBase> mainCategoryItems { get; set; } = new();
    public List<ShopItemBase> exchangeCategoryItems { get; set; } = new();

    public void OnSetupAppMode(AppMode appMode)
    {
        // ignored
    }
}
