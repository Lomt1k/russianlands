using Newtonsoft.Json;
using System.Text;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Units.Stats.StatEffects;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Potions;

[JsonObject]
public class AddResistancePotionData : PotionData
{
    public int physicalDamage { get; set; }
    public int fireDamage { get; set; }
    public int coldDamage { get; set; }
    public int lightningDamage { get; set; }
    public int turnsCount { get; set; }

    public AddResistancePotionData(int _id) : base(_id)
    {
    }

    public DamageInfo GetValues(GameSession session)
    {
        //TODO: Учитывать защитные навыки игрока?
        return new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
    }

    public override string GetDescription(GameSession sessionForValues, GameSession sessionForView)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(sessionForView, "potion_resistance_description", turnsCount));
        sb.AppendLine();
        sb.AppendLine(Localization.Get(sessionForView, "potion_description_protection_header"));
        sb.Append(GetValues(sessionForValues).GetCompactView());
        return sb.ToString();
    }

    public override void Apply(BattleTurn battleTurn, IBattleUnit unit)
    {
        var statEffect = new ExtraResistanceStatEffect(GetValues(unit.session), (byte)turnsCount);
        unit.unitStats.statEffects.Add(statEffect);
    }
}
