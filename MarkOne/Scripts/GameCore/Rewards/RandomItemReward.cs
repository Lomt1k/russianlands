using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Items.Generators;
using MarkOne.Scripts.GameCore.Sessions;
using System.Text;

namespace MarkOne.Scripts.GameCore.Rewards;

[JsonObject]
public class RandomItemReward : RewardBase
{
    public byte townhallLevel { get; set; } = 1;
    public Rarity rarity { get; set; }
    public ItemType itemType { get; set; } = ItemType.Any;
    public int count { get; set; } = 1;

    public override async Task<string?> AddReward(GameSession session)
    {
        try
        {
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                var item = itemType == ItemType.Any
                    ? ItemGenerationManager.GenerateItemWithSmartRandom(session, townhallLevel, rarity)
                    : ItemGenerationManager.GenerateItem(townhallLevel, itemType, rarity);
                session.player.inventory.ForceAddItem(item);

                if (i > 0)
                {
                    sb.AppendLine();
                }
                sb.Append(item.GetFullName(session).Bold());
            }            
            return sb.ToString();
        }
        catch (Exception ex)
        {
            await messageSender.SendErrorMessage(session.chatId, ex.Message);
            return null;
        }
    }

    public override string GetPossibleRewardsView(GameSession session)
    {
        return rarity.GetUnknownItemView(session, itemType, count);
    }
}
