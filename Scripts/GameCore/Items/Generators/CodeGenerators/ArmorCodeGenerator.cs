using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    /// <summary>
    /// For armor, boots and helmet
    /// </summary>
    internal class ArmorCodeGenerator : ItemCodeGeneratorBase
    {
        private List<Func<bool>> _options => new List<Func<bool>>
        {
            () => TryAppendProperty(PropertyType.IncreaseAttributeStrength),
            () => TryAppendProperty(PropertyType.IncreaseAttributeVitality),
            () => TryAppendProperty(PropertyType.IncreaseAttributeSorcery),
            () => TryAppendProperty(PropertyType.IncreaseAttributeLuck),
            () => TryAppendProperty(PropertyType.IncreaseMaxHealth),

            () => TryAppendAbility(AbilityType.RestoreHealth),

            () => { sb.Append("DRF"); return true; }, //damage resist fire
            () => { sb.Append("DRC"); return true; }, //damage resist cold
            () => { sb.Append("DRL"); return true; }, //damage resist lightning
        };

        private readonly string[] _nonCommonOptions = new[] { "DRF", "DRC", "DRL" };

        public ArmorCodeGenerator(ItemType _type, ItemRarity _rarity, ushort _level, int _basisPoints) : base(_type, _rarity, _level, _basisPoints)
        {
            if (type != ItemType.Armor && type != ItemType.Helmet && type != ItemType.Boots)
            {
                throw new ArgumentException($"ArmorCodeGenerator can not generate item with type '{_type}'");
            }
        }

        public override void AppendSpecificInfo()
        {
            var random = new Random();
            if (rarity != ItemRarity.Common)
            {
                var index = random.Next(_nonCommonOptions.Length);
                sb.Append(_nonCommonOptions[index]);
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
