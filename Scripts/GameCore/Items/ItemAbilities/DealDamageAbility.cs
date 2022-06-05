﻿using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    internal class DealDamageAbility : ItemAbilityBase
    {
        public override string debugDescription => "Наносит урон";
        public override AbilityType abilityType => AbilityType.DealDamage;
        public override ActivationType activationType => ActivationType.ByUser;
        public override bool isSupportLevelUp => true;

        // Не очень красиво, но так как эти поля меняются через рефлексию - проще так оставить
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
            var sb = new StringBuilder();
            sb.AppendLine($"{debugDescription}:");

            if (maxPhysicalDamage > 0)
                sb.AppendLine($"physical: {GetStringValue(DamageType.Physical)}");
            if (maxFireDamage > 0)
                sb.AppendLine($"fire: {GetStringValue(DamageType.Fire)}");
            if (maxColdDamage > 0)
                sb.AppendLine($"cold: {GetStringValue(DamageType.Cold)}");
            if (maxLightningDamage > 0)
                sb.AppendLine($"lightning: {GetStringValue(DamageType.Lightning)}");

            return sb.ToString();
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
