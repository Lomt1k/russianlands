using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators;

public class SwordCodeGenerator : ItemCodeGeneratorBase
{
    private static readonly string[] _rareOptions = new[] { "DF", "DC", "DL" };

    private static List<AbilityType> _availableKeywords => new List<AbilityType>
    {
        AbilityType.SwordBlockEveryTurnKeyword,
        AbilityType.AddArrowKeyword,
        AbilityType.StealManaKeyword,
        AbilityType.AdditionalFireDamageKeyword,
        AbilityType.AdditionalColdDamageKeyword,
        AbilityType.AdditionalLightningDamageKeyword,
        AbilityType.RageKeyword,
        AbilityType.FinishingKeyword,
        AbilityType.AbsorptionKeyword,
        AbilityType.StunKeyword,
        AbilityType.SanctionsKeyword,
    };

    public SwordCodeGenerator(ItemType _type, Rarity _rarity, int _townHallLevel) : base(_type, _rarity, _townHallLevel)
    {
        if (type != ItemType.Sword)
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
