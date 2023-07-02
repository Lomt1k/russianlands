using System;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Items.ItemAbilities;

namespace MarkOne.Scripts.GameCore.Items.Generators.CodeGenerators;

public class BowCodeGenerator : ItemCodeGeneratorBase
{
    private static readonly string[] _rareOptions = new[] { "DF", "DC", "DL" };

    private static List<AbilityType> _availableKeywords => new List<AbilityType>
    {
        AbilityType.BowLastShotKeyword,
        AbilityType.StealManaKeyword,
        AbilityType.AdditionalFireDamageKeyword,
        AbilityType.AdditionalColdDamageKeyword,
        AbilityType.AdditionalLightningDamageKeyword,
        AbilityType.FinishingKeyword,
        AbilityType.AbsorptionKeyword,
        AbilityType.StunKeyword,
        AbilityType.SanctionsKeyword,
    };

    public BowCodeGenerator(ItemType _type, Rarity _rarity, byte _townHallLevel, byte? grade = null) : base(_type, _rarity, _townHallLevel, grade)
    {
        if (type != ItemType.Bow)
        {
            throw new ArgumentException($"{GetType()} can not generate item with type '{_type}'");
        }
    }

    public override void AppendSpecificInfo()
    {
        var random = new Random();
        var blockedKeywords = new HashSet<AbilityType>();
        if (rarity != Rarity.Common)
        {
            var index = random.Next(_rareOptions.Length);
            var rareOption = _rareOptions[index];
            sb.Append(rareOption);

            var abilityToBlock = rareOption switch
            {
                "DF" => AbilityType.AdditionalFireDamageKeyword,
                "DC" => AbilityType.AdditionalColdDamageKeyword,
                "DL" => AbilityType.AdditionalLightningDamageKeyword,
                _ => AbilityType.None
            };
            blockedKeywords.Add(abilityToBlock);
        }

        var targetKeywordsCount = rarity.GetKeywordsCount();
        while (abilitiesCount < targetKeywordsCount)
        {
            var index = random.Next(_availableKeywords.Count);
            var keyword = _availableKeywords[index];
            if (!blockedKeywords.Contains(keyword))
            {
                AppendAbilityAsKeyword(_availableKeywords[index]);
            }
        }
    }

}
