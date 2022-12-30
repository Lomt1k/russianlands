using System.Text;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;

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

        public override string GetDescription(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format(Localization.Get(session, "potion_resistance_description"), turnsCount));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "potion_description_protection_header"));
            sb.Append(GetValues(session).GetCompactView());
            return sb.ToString();
        }
    }
}
