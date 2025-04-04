﻿using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities.Keywords;

public class SwordBlockKeywordAbility : ItemAbilityBase
{
    public override string debugDescription => "Блокирует входящий урон мечом (каждый ход)";

    public override AbilityType abilityType => AbilityType.SwordBlockEveryTurnKeyword;

    public int physicalDamage;
    public int fireDamage;
    public int coldDamage;
    public int lightningDamage;

    public DamageInfo GetValues()
    {
        return new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{debugDescription} (Вероятность {chanceToSuccessPercentage}%)");

        if (physicalDamage > 0)
        {
            sb.AppendLine();
            sb.Append($"physical: {physicalDamage}");
        }
        if (fireDamage > 0)
        {
            sb.AppendLine();
            sb.Append($"fire: {fireDamage}");
        }
        if (coldDamage > 0)
        {
            sb.AppendLine();
            sb.Append($"cold: {coldDamage}");
        }
        if (lightningDamage > 0)
        {
            sb.AppendLine();
            sb.Append($"lightning: {lightningDamage}");
        }

        return sb.ToString();
    }

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.KeywordSwordBlock;
    }

    public override string GetView(GameSession session)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localizations.Localization.Get(session, "ability_sword_block_percentage", chanceToSuccessPercentage));
        var damage = new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
        sb.Append(damage.GetCompactView());

        return sb.ToString();
    }

}
