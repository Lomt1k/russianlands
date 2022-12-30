using System.Text;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    public class HealthRestorePotionData : PotionData
    {
        public int healthAmount;

        public HealthRestorePotionData(int _id) : base(_id)
        {
        }

        public override string GetDescription(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "potion_health_description"));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "unit_view_health"));
            sb.Append($"{Emojis.stats[Stat.Health]} {healthAmount}");
            return sb.ToString();
        }
    }
}
