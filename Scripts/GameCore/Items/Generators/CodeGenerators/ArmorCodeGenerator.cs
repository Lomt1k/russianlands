using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    /// <summary>
    /// For armor, boots and helmet
    /// </summary>
    public class ArmorCodeGenerator : ItemCodeGeneratorBase
    {
        private List<Func<bool>> _options => new List<Func<bool>>
        {
            () => ForceAppendProperty(PropertyType.IncreaseAttributeStrength),
            () => ForceAppendProperty(PropertyType.IncreaseAttributeVitality),
            () => ForceAppendProperty(PropertyType.IncreaseAttributeSorcery),
            () => ForceAppendProperty(PropertyType.IncreaseAttributeLuck),
            () => ForceAppendProperty(PropertyType.IncreaseMaxHealth),

            () => { sb.Append("DF"); return true; }, //damage resist fire
            () => { sb.Append("DC"); return true; }, //damage resist cold
            () => { sb.Append("DL"); return true; }, //damage resist lightning
        };

        private readonly string[] _rareOptions = new[] { "DF", "DC", "DL" };

        public ArmorCodeGenerator(ItemType _type, Rarity _rarity, int _townHallLevel) : base(_type, _rarity, _townHallLevel)
        {
            if (type != ItemType.Armor && type != ItemType.Helmet && type != ItemType.Boots)
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

            int needOptionsCount = 0;
            switch (rarity)
            {
                case Rarity.Epic: needOptionsCount = 1; break;
                case Rarity.Legendary: needOptionsCount = 2; break;
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
