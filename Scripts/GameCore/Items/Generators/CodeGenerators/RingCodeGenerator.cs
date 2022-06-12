using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    internal class RingCodeGenerator : ItemCodeGeneratorBase
    {
        private List<Func<bool>> _options => new List<Func<bool>>
        {
            () => TryAppendProperty(PropertyType.IncreaseAttributeStrength),
            () => TryAppendProperty(PropertyType.IncreaseAttributeVitality),
            () => TryAppendProperty(PropertyType.IncreaseAttributeSorcery),
            () => TryAppendProperty(PropertyType.IncreaseAttributeLuck),
            () => TryAppendProperty(PropertyType.IncreaseMaxHealth),

            //() => TryAppendAbility(AbilityType.RestoreHealth),
        };

        public RingCodeGenerator(ItemType _type, ItemRarity _rarity, ushort _level, int _basisPoints) : base(_type, _rarity, _level, _basisPoints)
        {
            if (type != ItemType.Ring)
            {
                throw new ArgumentException($"{GetType()} can not generate item with type '{_type}'");
            }
            if (rarity == ItemRarity.Common)
            {
                throw new ArgumentException($"{GetType()} can not generate items with '{_rarity}' rarity");
            }
        }

        public override void AppendSpecificInfo()
        {
            var random = new Random();

            int needOptionsCount = 0;
            switch (rarity)
            {
                case ItemRarity.Rare: needOptionsCount = 1; break;
                case ItemRarity.Epic: needOptionsCount = 2; break;
                case ItemRarity.Legendary: needOptionsCount = 3; break;
            }

            while (needOptionsCount > 0)
            {
                var index = random.Next(_options.Count);
                bool result = _options[index]();
                if (result)
                {
                    needOptionsCount--;
                }
            }
        }


    }
}
