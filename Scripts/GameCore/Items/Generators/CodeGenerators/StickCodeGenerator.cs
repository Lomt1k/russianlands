using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    internal class StickCodeGenerator : ItemCodeGeneratorBase
    {
        private List<Func<bool>> _options => new List<Func<bool>>
        {
            () => TryAppendProperty(PropertyType.IncreaseAttributeStrength),
            () => TryAppendProperty(PropertyType.IncreaseAttributeVitality),
            () => TryAppendProperty(PropertyType.IncreaseAttributeSorcery),
            () => TryAppendProperty(PropertyType.IncreaseAttributeLuck),
            () => TryAppendProperty(PropertyType.IncreaseMaxHealth),

            () => { sb.Append("DF"); return true; }, //damage fire
            () => { sb.Append("DC"); return true; }, //damage cold
            () => { sb.Append("DL"); return true; }, //damage lightning
        };

        private readonly string[] _mainOption = new[] { "DF", "DC", "DL" };

        public StickCodeGenerator(ItemType _type, ItemRarity _rarity, ushort _level, int _basisPoints) : base(_type, _rarity, _level, _basisPoints)
        {
            if (type != ItemType.Stick)
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

            var index = random.Next(_mainOption.Length);
            sb.Append(_mainOption[index]);

            int needOptionsCount = 0;
            switch (rarity)
            {
                case ItemRarity.Epic: needOptionsCount = 1; break;
                case ItemRarity.Legendary: needOptionsCount = 2; break;
            }

            while (needOptionsCount > 0)
            {
                index = random.Next(_options.Count);
                bool result = _options[index]();
                if (result)
                {
                    needOptionsCount--;
                }
            }
        }


    }
}
