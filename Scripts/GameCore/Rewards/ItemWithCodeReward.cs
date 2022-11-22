using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    public class ItemWithCodeReward : RewardBase
    {
        [JsonProperty]
        public string itemCode = string.Empty;

        public override async Task<string> AddReward(GameSession session)
        {
            try
            {
                var item = new InventoryItem(itemCode);
                var addedItem = session.player.inventory.TryAddItem(item);
                if (addedItem != null)
                {
                    return $"<b>{addedItem.GetFullName(session)}</b>";
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                await Bot.TelegramBot.instance.messageSender.SendErrorMessage(session.chatId, ex.Message);
                return string.Empty;
            }
        }

    }
}
