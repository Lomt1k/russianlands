using System;
using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities;

public class DealDamageAbility : ItemAbilityBase
{
    public override string debugDescription => "Наносит урон";
    public override AbilityType abilityType => AbilityType.DealDamage;

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
        var random = new Random();
        return new DamageInfo(
            random.Next(minPhysicalDamage, maxPhysicalDamage + 1),
            random.Next(minFireDamage, maxFireDamage + 1),
            random.Next(minColdDamage, maxColdDamage + 1),
            random.Next(minLightningDamage, maxLightningDamage + 1)
            );
    }

    public DamageInfo GetAverageValues()
    {
        return new DamageInfo(
            (minPhysicalDamage + maxPhysicalDamage) / 2,
            (minFireDamage + maxFireDamage) / 2,
            (minColdDamage + maxColdDamage) / 2,
            (minLightningDamage + maxLightningDamage) / 2
            );
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
        sb.Append(Localizations.Localization.Get(session, "ability_deals_damage"));
        if (minPhysicalDamage > 0)
        {
            sb.AppendLine();
            sb.Append(Emojis.StatPhysicalDamage + GetStringValue(DamageType.Physical));
        }
        if (minFireDamage > 0)
        {
            sb.AppendLine();
            sb.Append(Emojis.StatFireDamage + GetStringValue(DamageType.Fire));
        }
        if (minColdDamage > 0)
        {
            sb.AppendLine();
            sb.Append(Emojis.StatColdDamage + GetStringValue(DamageType.Cold));
        }
        if (minLightningDamage > 0)
        {
            sb.AppendLine();
            sb.Append(Emojis.StatLightningDamage + GetStringValue(DamageType.Lightning));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Только для свитков, так как у них всегда только один вид урона
    /// </summary>
    public string GetSimpleView(GameSession session)
    {
        var sb = new StringBuilder();
        sb.Append(Localizations.Localization.Get(session, "ability_deals_damage") + ": ");
        if (minPhysicalDamage > 0)
        {
            sb.Append(Emojis.StatPhysicalDamage + GetStringValue(DamageType.Physical));
        }
        if (minFireDamage > 0)
        {
            sb.Append(Emojis.StatFireDamage + GetStringValue(DamageType.Fire));
        }
        if (minColdDamage > 0)
        {
            sb.Append(Emojis.StatColdDamage + GetStringValue(DamageType.Cold));
        }
        if (minLightningDamage > 0)
        {
            sb.Append(Emojis.StatLightningDamage + GetStringValue(DamageType.Lightning));
        }

        return sb.ToString();
    }

    private string GetStringValue(DamageType damageType)
    {
        var minDamage = 0;
        var maxDamage = 0;
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

    /// <summary>
    /// Только для свитков!
    /// </summary>
    public DamageType GetDamageTypeForScroll()
    {
        if (minFireDamage > 0) return DamageType.Fire;
        if (minColdDamage > 0) return DamageType.Cold;
        if (minLightningDamage > 0) return DamageType.Lightning;
        return DamageType.Physical;
    }

    public override void ApplySkillLevel(byte level)
    {
        IncreaseByPercents(ref minPhysicalDamage, level);
        IncreaseByPercents(ref maxPhysicalDamage, level);
        IncreaseByPercents(ref minFireDamage, level);
        IncreaseByPercents(ref maxFireDamage, level);
        IncreaseByPercents(ref minColdDamage, level);
        IncreaseByPercents(ref maxColdDamage, level);
        IncreaseByPercents(ref minLightningDamage, level);
        IncreaseByPercents(ref maxLightningDamage, level);
    }

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        if (maxFireDamage > 0)
        {
            yield return ItemStatIcon.FireDamage;
        }
        if (maxColdDamage > 0)
        {
            yield return ItemStatIcon.ColdDamage;
        }
        if (maxLightningDamage > 0)
        {
            yield return ItemStatIcon.LightningDamage;
        }
    }

}
