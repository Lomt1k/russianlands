using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    internal class ShieldCodeGenerator : ItemCodeGeneratorBase
    {
        private List<Func<bool>> _options => new List<Func<bool>>
        {
            () => TryAppendProperty(PropertyType.IncreaseAttributeStrength),
            () => TryAppendProperty(PropertyType.IncreaseAttributeVitality),
            () => TryAppendProperty(PropertyType.IncreaseAttributeSorcery),
            () => TryAppendProperty(PropertyType.IncreaseAttributeLuck),
            () => TryAppendProperty(PropertyType.IncreaseMaxHealth),

            //() => TryAppendAbility(AbilityType.RestoreHealth),

            () => { sb.Append("DF"); return true; }, //block damage fire
            () => { sb.Append("DC"); return true; }, //block damage cold
            () => { sb.Append("DL"); return true; }, //block damage lightning
        };

        private readonly string[] _rareOptions = new[] { "DF", "DC", "DL" };

        public ShieldCodeGenerator(ItemType _type, ItemRarity _rarity, ushort _level, int _basisPoints) : base(_type, _rarity, _level, _basisPoints)
        {
            if (type != ItemType.Shield)
            {
                throw new ArgumentException($"{GetType()} can not generate item with type '{_type}'");
            }
        }

        public override void AppendSpecificInfo()
        {
            var random = new Random();
            if (rarity != ItemRarity.Common)
            {
                var index = random.Next(_rareOptions.Length);
                sb.Append(_rareOptions[index]);
            }

            int needOptionsCount = 0;
            switch (rarity)
            {
                case ItemRarity.Epic: needOptionsCount = 1; break;
                case ItemRarity.Legendary: needOptionsCount = 2; break;
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
