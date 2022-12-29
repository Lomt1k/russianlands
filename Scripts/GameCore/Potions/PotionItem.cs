using Newtonsoft.Json;
using System;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    [JsonObject]
    public class PotionItem
    {
        [JsonProperty("id")]
        private int _id;
        [JsonProperty("t")]
        private long _preparationTime;

        [JsonIgnore]
        public int id => _id;
        [JsonIgnore]
        public long preparationTime => _preparationTime;

        public PotionItem(int id, long preparationTime)
        {
            _id = id;
            _preparationTime = preparationTime;
        }

        public PotionData GetData()
        {
            return GameDataBase.GameDataBase.instance.potions[_id];
        }

        public bool IsReady()
        {
            return DateTime.UtcNow.Ticks > _preparationTime;
        }

    }
}
