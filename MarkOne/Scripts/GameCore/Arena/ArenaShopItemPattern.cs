using MarkOne.Scripts.GameCore.Items;
using Newtonsoft.Json;

namespace MarkOne.Scripts.GameCore.Arena;
[JsonObject]
public class ArenaShopItemPattern
{
    public Rarity rarity { get; set; }
    public int count { get; set; } = 1;
    public int chipsPrice { get; set; }

}
