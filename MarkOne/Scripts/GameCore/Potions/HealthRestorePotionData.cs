using Newtonsoft.Json;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Potions;

[JsonObject]
public class HealthRestorePotionData : PotionData
{
    public int healthAmount { get; set; }

    public HealthRestorePotionData(int _id) : base(_id)
    {
    }

    public override string GetDescription(GameSession sessionForValues, GameSession sessionForView)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(sessionForView, "potion_health_description"));
        sb.AppendLine();
        sb.AppendLine(Localization.Get(sessionForView, "unit_view_health"));
        sb.Append(Emojis.StatHealth + healthAmount.ToString());
        return sb.ToString();
    }

    public override void Apply(BattleTurn battleTurn, IBattleUnit unit)
    {
        unit.unitStats.RestoreHealth(healthAmount);
    }

}
