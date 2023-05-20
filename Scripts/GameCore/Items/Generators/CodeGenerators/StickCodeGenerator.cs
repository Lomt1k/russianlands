using System;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Items.ItemAbilities;

namespace MarkOne.Scripts.GameCore.Items.Generators.CodeGenerators;

public class StickCodeGenerator : ItemCodeGeneratorBase
{
    private static readonly string[] _mainOption = new[] { "DF", "DC", "DL" };

    private static List<AbilityType> _availableKeywords => new List<AbilityType>
    {
        AbilityType.StealManaKeyword,
        AbilityType.AdditionalFireDamageKeyword,
        AbilityType.AdditionalColdDamageKeyword,
        AbilityType.AdditionalLightningDamageKeyword,
        AbilityType.RageKeyword,
        AbilityType.FinishingKeyword,
        AbilityType.AbsorptionKeyword,
        AbilityType.AddManaKeyword,
        AbilityType.StunKeyword,
        AbilityType.SanctionsKeyword,
    };

    public StickCodeGenerator(ItemType _type, Rarity _rarity, byte _townHallLevel, byte? grade = null) : base(_type, _rarity, _townHallLevel, grade)
    {
        if (type != ItemType.Stick)
        {
            throw new ArgumentException($"{GetType()} can not generate item with type '{_type}'");
        }
    }

    public override void AppendSpecificInfo()
    {
        var random = new Random();
        var blockedKeywords = new HashSet<AbilityType>();
        var index = random.Next(_mainOption.Length);
        var mainOption = _mainOption[index];
        sb.Append(mainOption);

        var abilityToBlock = mainOption switch
        {
            "DF" => AbilityType.AdditionalFireDamageKeyword,
            "DC" => AbilityType.AdditionalColdDamageKeyword,
            "DL" => AbilityType.AdditionalLightningDamageKeyword,
            _ => AbilityType.None
        };
        blockedKeywords.Add(abilityToBlock);

        var targetKeywordsCount = rarity.GetKeywordsCount();
        while (abilitiesCount < targetKeywordsCount)
        {
            index = random.Next(_availableKeywords.Count);
            var keyword = _availableKeywords[index];
            if (!blockedKeywords.Contains(keyword))
            {
                AppendAbilityAsKeyword(_availableKeywords[index]);
            }
        }
    }


}
