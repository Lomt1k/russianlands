using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Sessions;
using System.Runtime.Serialization;

namespace MarkOne.Scripts.GameCore.Rewards;

[JsonObject]
public class ItemWithCodeReward : RewardBase
{
    public string itemCode { get; set; } = string.Empty;

    [JsonIgnore]
    public InventoryItem itemTemplate { get; private set; }

    [JsonConstructor]
    public ItemWithCodeReward()
    {
        
    }

    public ItemWithCodeReward(string _itemCode)
    {
        itemCode = _itemCode;
        TryParseItemCode();
    }

    public ItemWithCodeReward(InventoryItem _itemTemplate)
    {
        itemTemplate = _itemTemplate;
        itemCode = _itemTemplate.id;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        TryParseItemCode();
    }

    private void TryParseItemCode()
    {
        try
        {
            itemTemplate = new InventoryItem(itemCode);
        }
        catch (Exception ex)
        {
            Program.logger.Error(ex);
        }
    }

    public override async Task<string?> AddReward(GameSession session)
    {
        try
        {
            var item = itemTemplate.Clone();
            var success = session.player.inventory.TryAddItem(item);
            return success ? item.GetFullName(session).Bold() : string.Empty;
        }
        catch (Exception ex)
        {
            await messageSender.SendErrorMessage(session.chatId, ex.Message);
            return null;
        }
    }

    public override string GetPossibleRewardsView(GameSession session)
    {
        return itemTemplate.GetFullName(session).Bold();
    }
}
