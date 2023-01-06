using System.Text;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.GameCore.Units.Stats.StatEffects;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    public class AddResistancePotionData : PotionData
    {
        public int physicalDamage;
        public int fireDamage;
        public int coldDamage;
        public int lightningDamage;
        public int turnsCount;

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
            sb.AppendLine(string.Format(Localization.Get(sessionForView, "potion_resistance_description"), turnsCount));
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
}
