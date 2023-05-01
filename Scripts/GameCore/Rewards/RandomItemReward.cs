using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.Generators;

namespace TextGameRPG.Scripts.GameCore.Rewards
{
    [JsonObject]
    public class RandomItemReward : RewardBase
    {
        public byte townhallLevel { get; set; } = 1;
        public Rarity rarity { get; set; }

        public override async Task<string?> AddReward(GameSession session)
        {
            try
            {
                var item = ItemGenerationManager.GenerateItemWithSmartRandom(session, townhallLevel, rarity);
                var success = session.player.inventory.TryAddItem(item);
                return success ? item.GetFullName(session).Bold() : string.Empty;
            }
            catch (Exception ex)
            {
                await messageSender.SendErrorMessage(session.chatId, ex.Message);
                return null;
            }
        }

    }
}
