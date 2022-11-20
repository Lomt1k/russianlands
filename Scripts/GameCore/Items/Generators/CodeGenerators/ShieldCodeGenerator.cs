using System;
using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    public class ShieldCodeGenerator : ItemCodeGeneratorBase
    {
        private List<Func<bool>> _options => new List<Func<bool>>
        {
            () => { sb.Append("DF"); return true; }, //block damage fire
            () => { sb.Append("DC"); return true; }, //block damage cold
            () => { sb.Append("DL"); return true; }, //block damage lightning
        };

        public ShieldCodeGenerator(ItemType _type, Rarity _rarity, int _townHallLevel) : base(_type, _rarity, _townHallLevel)
        {
            if (type != ItemType.Shield)
            {
                throw new ArgumentException($"{GetType()} can not generate item with type '{_type}'");
            }
        }

        public override void AppendSpecificInfo()
        {
            var random = new Random();
            int needOptionsCount = 0;
            switch (rarity)
            {
                case Rarity.Rare: needOptionsCount = 1; break;
                case Rarity.Epic: needOptionsCount = 2; break;
                case Rarity.Legendary: needOptionsCount = 3; break;
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
