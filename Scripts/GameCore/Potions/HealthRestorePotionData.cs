using System.Text;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    public class HealthRestorePotionData : PotionData
    {
        public int healthAmount;

        public HealthRestorePotionData(int _id) : base(_id)
        {
        }

        public override string GetDescription(GameSession sessionForValues, GameSession sessionForView)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(sessionForView, "potion_health_description"));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(sessionForView, "unit_view_health"));
            sb.Append($"{Emojis.stats[Stat.Health]} {healthAmount}");
            return sb.ToString();
        }

        public override void Apply(BattleTurn battleTurn, IBattleUnit unit)
        {
            unit.unitStats.RestoreHealth(healthAmount);
        }

    }
}
