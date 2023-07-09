using MarkOne.Scripts.GameCore.Services.GameData;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Shop;
public class ShopSettings : IGameDataWithId<byte>
{
    public byte id { get; set; }
    public List<ShopItemBase> premiumCategoryItems { get; set; } = new();
    public List<ShopItemBase> lootboxCategoryItems { get; set; } = new();
    public List<ShopItemBase> diamondsCategoryItems { get; set; } = new();

    public void OnBotAppStarted()
    {
        // ignored
    }
}
