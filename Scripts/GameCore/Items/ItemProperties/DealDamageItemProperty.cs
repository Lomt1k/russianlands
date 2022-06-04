using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    public struct DamageInfo
    {
        public int physicalDamage { get; }
        public int fireDamage { get; }
        public int coldDamage { get; }
        public int lightningDamage { get; }

        public DamageInfo(int _physicalDamage, int _fireDamage, int _coldDamage, int _lightningDamage)
        {
            physicalDamage = _physicalDamage;
            fireDamage = _fireDamage;
            coldDamage = _coldDamage;
            lightningDamage = _lightningDamage;
        }
    }

    internal class DealDamageItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Наносит урон";
        public override ItemPropertyType propertyType => ItemPropertyType.DealDamage;
        public override bool isSupportLevelUp => true;

        // Не очень красиво, но так как поля пропертей меняются через рефлексию - проще так оставить
        public int minPhysicalDamage;
        public int maxPhysicalDamage;
        public int minFireDamage;
        public int maxFireDamage;
        public int minColdDamage;
        public int maxColdDamage;
        public int minLightningDamage;
        public int maxLightningDamage;

        public DamageInfo GetRandomValues()
        {
            return new DamageInfo(
                Randomizer.random.Next(minPhysicalDamage, maxPhysicalDamage + 1),
                Randomizer.random.Next(minFireDamage, maxFireDamage + 1),
                Randomizer.random.Next(minColdDamage, maxColdDamage + 1),
                Randomizer.random.Next(minLightningDamage, maxLightningDamage + 1));
        }

        public override void ApplyItemLevel(byte level)
        {
            float minDamageBonusPerLevel = minPhysicalDamage / 10 > 0 ? (float)minPhysicalDamage / 10 : 1;
            float maxDamageBonusPerLevel = maxPhysicalDamage / 10 > 0 ? (float)maxPhysicalDamage / 10 : 1;
            minPhysicalDamage += (int)(minDamageBonusPerLevel * level);
            maxPhysicalDamage += (int)(maxDamageBonusPerLevel * level);

            minDamageBonusPerLevel = minFireDamage / 10 > 0 ? (float)minFireDamage / 10 : 1;
            maxDamageBonusPerLevel = maxFireDamage / 10 > 0 ? (float)maxFireDamage / 10 : 1;
            minFireDamage += (int)(minDamageBonusPerLevel * level);
            maxFireDamage += (int)(maxDamageBonusPerLevel * level);

            minDamageBonusPerLevel = minColdDamage / 10 > 0 ? (float)minColdDamage / 10 : 1;
            maxDamageBonusPerLevel = maxColdDamage / 10 > 0 ? (float)maxColdDamage / 10 : 1;
            minColdDamage += (int)(minDamageBonusPerLevel * level);
            maxColdDamage += (int)(maxDamageBonusPerLevel * level);

            minDamageBonusPerLevel = minLightningDamage / 10 > 0 ? (float)minLightningDamage / 10 : 1;
            maxDamageBonusPerLevel = maxLightningDamage / 10 > 0 ? (float)maxLightningDamage / 10 : 1;
            minLightningDamage += (int)(minDamageBonusPerLevel * level);
            maxLightningDamage += (int)(maxDamageBonusPerLevel * level);
        }

        public override string ToString()
        {
            return $"{debugDescription}:"
                + $"\nphysical: {GetStringValue(DamageType.Physical)}"
                + $"\nfire: {GetStringValue(DamageType.Fire)}"
                + $"\ncold: {GetStringValue(DamageType.Cold)}"
                + $"\nlightning: {GetStringValue(DamageType.Lightning)}";
        }

        public override string GetView(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Localization.Get(session, "item_view_deals_damage"));
            if (minPhysicalDamage > 0)
            {
                sb.AppendLine($"{Emojis.stats[Stat.PhysicalDamage]} {GetStringValue(DamageType.Physical)}");
            }
            if (minFireDamage > 0)
            {
                sb.AppendLine($"{Emojis.stats[Stat.FireDamage]} {GetStringValue(DamageType.Fire)}");
            }
            if (minColdDamage > 0)
            {
                sb.AppendLine($"{Emojis.stats[Stat.ColdDamage]} {GetStringValue(DamageType.Cold)}");
            }
            if (minLightningDamage > 0)
            {
                sb.AppendLine($"{Emojis.stats[Stat.LightningDamage]} {GetStringValue(DamageType.Lightning)}");
            }

            return sb.ToString();
        }

        private string GetStringValue(DamageType damageType)
        {
            int minDamage = 0;
            int maxDamage = 0;
            switch (damageType)
            {
                case DamageType.Physical:
                    minDamage = minPhysicalDamage;
                    maxDamage = maxPhysicalDamage;
                    break;
                case DamageType.Fire:
                    minDamage = minFireDamage;
                    maxDamage = maxFireDamage;
                    break;
                case DamageType.Cold:
                    minDamage = minColdDamage;
                    maxDamage = maxColdDamage;
                    break;
                case DamageType.Lightning:
                    minDamage = minLightningDamage;
                    maxDamage = maxLightningDamage;
                    break;
            }

            return minDamage == maxDamage ? minDamage.ToString() : $"{minDamage} - {maxDamage}";
        }

    }
}
