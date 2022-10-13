using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    public class DamageResistProperty : ItemPropertyBase
    {
        public override string debugDescription => "Сопротивление урону";
        public override PropertyType propertyType => PropertyType.DamageResist;
        public override bool isSupportLevelUp => true;

        public int physicalDamage;
        public int fireDamage;
        public int coldDamage;
        public int lightningDamage;

        public override void ApplyItemLevel(byte level)
        {
            IncreaseByTenPercentByLevel(ref physicalDamage, level);
            IncreaseByTenPercentByLevel(ref fireDamage, level);
            IncreaseByTenPercentByLevel(ref coldDamage, level);
            IncreaseByTenPercentByLevel(ref lightningDamage, level);
        }

        public DamageInfo GetValues()
        {
            return new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{debugDescription}:");

            if (physicalDamage > 0)
                sb.AppendLine($"physical: {physicalDamage}");
            if (fireDamage > 0)
                sb.AppendLine($"fire: {fireDamage}");
            if (coldDamage > 0)
                sb.AppendLine($"cold: {coldDamage}");
            if (lightningDamage > 0)
                sb.AppendLine($"lightning: {lightningDamage}");

            return sb.ToString();
        }

        public override string GetView(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localizations.Localization.Get(session, "property_damage_resist"));
            var damage = new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
            sb.Append(damage.GetCompactView());

            return sb.ToString();
        }

    }
}
