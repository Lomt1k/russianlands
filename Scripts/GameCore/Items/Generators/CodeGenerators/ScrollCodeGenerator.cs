using System;
using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    internal class ScrollCodeGenerator : ItemCodeGeneratorBase
    {
        private List<Func<bool>> _options => new List<Func<bool>>
        {
            () => AppendAnyFireSpell(),
            () => AppendAnyColdSpell(),
            () => AppendAnyLightningSpell(),
        };

        public ScrollCodeGenerator(ItemType _type, ItemRarity _rarity, ushort _level, int _basisPoints) : base(_type, _rarity, _level, _basisPoints)
        {
            if (type != ItemType.Scroll)
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
            bool result = false;

            while (!result)
            {
                var index = random.Next(_options.Count);
                result = _options[index]();
            }
        }

        private bool AppendAnyFireSpell()
        {
            sb.Append("DF");
            var mana = new Random().Next(2, 6);
            AppendRequiredMana(mana);
            return true;
        }

        private bool AppendAnyColdSpell()
        {
            sb.Append("DC");
            var mana = new Random().Next(2, 6);
            AppendRequiredMana(mana);
            return true;
        }

        private bool AppendAnyLightningSpell()
        {
            sb.Append("DL");
            var mana = new Random().Next(2, 6);
            AppendRequiredMana(mana);
            return true;
        }

        private void AppendRequiredMana(int mana)
        {
            sb.Append($"M{mana}");
        }


    }
}
