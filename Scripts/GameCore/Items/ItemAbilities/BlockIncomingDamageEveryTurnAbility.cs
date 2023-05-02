using System.Text;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Items.ItemAbilities;

public class BlockIncomingDamageEveryTurnAbility : ItemAbilityBase
{
    public override string debugDescription => "Блокирует входящий урон (каждый ход)";

    public override AbilityType abilityType => AbilityType.BlockIncomingDamageEveryTurn;

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

    public override string GetView(GameSession session)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localizations.Localization.Get(session, "ability_block_damage_percentage", chanceToSuccessPercentage));
        var damage = new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
        sb.Append(damage.GetCompactView());

        return sb.ToString();
    }

    public override void ApplySkillLevel(byte level)
    {
        IncreaseByPercents(ref physicalDamage, level);
        IncreaseByPercents(ref fireDamage, level);
        IncreaseByPercents(ref coldDamage, level);
        IncreaseByPercents(ref lightningDamage, level);
    }

}
