using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    public class SwordCodeGenerator : ItemCodeGeneratorBase
    {
        private static string[] _rareOptions = new[] { "DF", "DC", "DL" };

        private static List<AbilityType> _availableKeywords => new List<AbilityType>
        {
            AbilityType.SwordBlockEveryTurnKeyword,
            AbilityType.AddArrowKeyword,
            AbilityType.StealManaKeyword,
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
            if (rarity != Rarity.Common)
            {
                var index = random.Next(_rareOptions.Length);
                sb.Append(_rareOptions[index]);
            }

            int targetKeywordsCount = rarity.GetKeywordsCount();
            while (abilitiesCount < targetKeywordsCount)
            {
                var index = random.Next(_availableKeywords.Count);
                AppendAbilityAsKeyword(_availableKeywords[index]);
            }
        }

    }
}
