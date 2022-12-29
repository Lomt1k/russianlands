using Newtonsoft.Json;
using System.Collections.Generic;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    [JsonObject]
    public class PlayerPotions
    {
        [JsonProperty("p")]
        public List<PotionItem> potions { get; private set; } = new List<PotionItem>();

        [JsonIgnore]
        public GameSession session { get; private set; }
        [JsonIgnore]
        public int maxCount => session.profile.data.IsPremiumActive() ? 12 : 6;
        [JsonIgnore]
        public bool isFull => potions.Count >= maxCount;

        public void SetupSession(GameSession _session)
        {
            session = _session;
        }

        public bool HasReadyPotions()
        {
            foreach (var item in potions)
            {
                if (item.IsReady())
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<PotionItem> GetReadyPotions()
        {
            foreach (var item in potions)
            {
                if (item.IsReady())
                {
                    yield return item;
                }
            }
        }

    }
}
