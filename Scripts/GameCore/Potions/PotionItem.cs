using Newtonsoft.Json;
using System;
using System.Text;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;

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

        public void BoostProduction()
        {
            _preparationTime = DateTime.UtcNow.Ticks - 1;
        }

        public int GetBoostPriceInDiamonds()
        {
            if (IsReady())
                return 0;

            var timeSpan = new DateTime(preparationTime) - DateTime.UtcNow;
            var seconds = (int)timeSpan.TotalSeconds;
            var requiredDiamonds = ResourceHelper.CalculatePotionCraftBoostPriceInDiamonds(seconds);
            return requiredDiamonds;
        }

        public string GetName(GameSession session)
        {
            return GetData().GetName(session);
        }

        public string GetNameForList(GameSession session)
        {
            if (IsReady())
            {
                return GetData().GetName(session);
            }

            var timeSpan = new DateTime(_preparationTime) - DateTime.UtcNow;
            return timeSpan.GetShortView(session) + " | " + GetData().GetNameWithoutIcon(session);
        }

        public string GetView(GameSession session)
        {
            var sb = new StringBuilder();
            var data = GetData();
            sb.AppendLine(data.GetName(session));
            sb.AppendLine();
            sb.AppendLine(data.GetDescription(session, session));

            if (!IsReady())
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_potions_in_production_header"));
                var timeSpan = new DateTime(_preparationTime) - DateTime.UtcNow;
                sb.Append(timeSpan.GetView(session));
            }
            return sb.ToString();
        }

    }
}
