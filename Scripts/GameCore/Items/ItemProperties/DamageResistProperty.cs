using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class DamageResistProperty : ItemPropertyBase
    {
        public override string debugDescription => "Сопротивление урону";
        public override ItemPropertyType propertyType => ItemPropertyType.DamageResist;
        public override bool isSupportLevelUp => true;

        public int physicalDamage;
        public int fireDamage;
        public int coldDamage;
        public int lightningDamage;

        public override void ApplyItemLevel(byte level)
        {
            float bonusPerLevel = physicalDamage / 10 > 0 ? (float)physicalDamage / 10 : 1;
            physicalDamage += (int)(bonusPerLevel * level);

            bonusPerLevel = fireDamage / 10 > 0 ? (float)fireDamage / 10 : 1;
            fireDamage += (int)(bonusPerLevel * level);

            bonusPerLevel = coldDamage / 10 > 0 ? (float)coldDamage / 10 : 1;
            coldDamage += (int)(bonusPerLevel * level);

            bonusPerLevel = lightningDamage / 10 > 0 ? (float)lightningDamage / 10 : 1;
            lightningDamage += (int)(bonusPerLevel * level);
        }

        public DamageInfo GetValues()
        {
            return new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
        }

        public override string ToString()
        {
            return $"{debugDescription}:"
                + $"\nphysical: {physicalDamage}"
                + $"\nfire: {fireDamage}"
                + $"\ncold: {coldDamage}"
                + $"\nlightning: {lightningDamage}";
        }

        public override string GetView(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Localization.Get(session, "item_view_protects_from_damage"));
            if (physicalDamage > 0)
            {
                sb.AppendLine($"{Emojis.stats[Stat.PhysicalDamage]} {physicalDamage}");
            }
            if (fireDamage > 0)
            {
                sb.AppendLine($"{Emojis.stats[Stat.FireDamage]} {fireDamage}");
            }
            if (coldDamage > 0)
            {
                sb.AppendLine($"{Emojis.stats[Stat.ColdDamage]} {coldDamage}");
            }
            if (lightningDamage > 0)
            {
                sb.AppendLine($"{Emojis.stats[Stat.LightningDamage]} {lightningDamage}");
            }

            return sb.ToString();
        }

    }
}
