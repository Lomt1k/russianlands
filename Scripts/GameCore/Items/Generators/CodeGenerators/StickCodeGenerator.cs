﻿using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    public class StickCodeGenerator : ItemCodeGeneratorBase
    {
        private List<Func<bool>> _options => new List<Func<bool>>
        {
            () => ForceAppendProperty(PropertyType.IncreaseAttributeStrength),
            () => ForceAppendProperty(PropertyType.IncreaseAttributeVitality),
            () => ForceAppendProperty(PropertyType.IncreaseAttributeSorcery),
            () => ForceAppendProperty(PropertyType.IncreaseAttributeLuck),
            () => ForceAppendProperty(PropertyType.IncreaseMaxHealth),

            () => { sb.Append("DF"); return true; }, //damage fire
            () => { sb.Append("DC"); return true; }, //damage cold
            () => { sb.Append("DL"); return true; }, //damage lightning
        };

        private readonly string[] _mainOption = new[] { "DF", "DC", "DL" };

        public StickCodeGenerator(ItemType _type, Rarity _rarity, int _townHallLevel) : base(_type, _rarity, _townHallLevel)
        {
            if (type != ItemType.Stick)
            {
                throw new ArgumentException($"{GetType()} can not generate item with type '{_type}'");
            }
        }

        public override void AppendSpecificInfo()
        {
            var random = new Random();

            var index = random.Next(_mainOption.Length);
            sb.Append(_mainOption[index]);
            sb.Append("C3"); // required charge 3

            int needOptionsCount = 0;
            switch (rarity)
            {
                case Rarity.Rare: needOptionsCount = 1; break;
                case Rarity.Epic: needOptionsCount = 2; break;
                case Rarity.Legendary: needOptionsCount = 3; break;
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
