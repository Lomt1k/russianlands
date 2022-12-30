using System.Text;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    public class AddDamagePotionData : PotionData
    {
        public int physicalDamage;
        public int fireDamage;
        public int coldDamage;
        public int lightningDamage;

        public AddDamagePotionData(int _id) : base(_id)
        {
        }

        public DamageInfo GetValues(GameSession session)
        {
            //TODO: Учитывать атакующие навыки игрока?
            return new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
        }

        public override string GetDescription(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "potion_add_damage_description"));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "potion_description_damage_header"));
            sb.Append(GetValues(session).GetCompactView());
            return sb.ToString();
        }

    }
}
