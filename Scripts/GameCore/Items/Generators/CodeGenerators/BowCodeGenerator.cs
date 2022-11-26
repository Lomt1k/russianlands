using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    public class BowCodeGenerator : ItemCodeGeneratorBase
    {
        private static List<Action<StringBuilder>> _rareOptions => new List<Action<StringBuilder>>
        {
            sb => sb.Append("DF"), //damage fire
            sb => sb.Append("DC"), //damage cold
            sb => sb.Append("DL"), //damage lightning
        };

        private static List<AbilityType> _availableKeywords => new List<AbilityType>
        {
            AbilityType.BowLastShotKeyword,
        };

        public BowCodeGenerator(ItemType _type, Rarity _rarity, int _townHallLevel) : base(_type, _rarity, _townHallLevel)
        {
            if (type != ItemType.Bow)
            {
                throw new ArgumentException($"{GetType()} can not generate item with type '{_type}'");
            }
        }

        public override void AppendSpecificInfo()
        {
            var random = new Random();
            if (rarity != Rarity.Common)
            {
                var index = random.Next(_rareOptions.Count);
                _rareOptions[index](sb);
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
