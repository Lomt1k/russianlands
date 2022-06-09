using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    using ItemAbilities;
    using ItemProperties;
    using System.Linq;

    internal abstract partial class ItemDataGeneratorBase
    {
        protected abstract ItemType type { get; }

        protected ItemRarity rarity { get; }
        protected ushort level { get; }
        protected int basisPoint { get; }
        protected byte requiredChange { get; set; }

        private Dictionary<AbilityType, ItemAbilityBase> _abilities = new Dictionary<AbilityType, ItemAbilityBase>();
        private Dictionary<PropertyType, ItemPropertyBase> _properties = new Dictionary<PropertyType, ItemPropertyBase>();

        public ItemDataGeneratorBase(ItemRarity _rarity, ushort _level, int _basisPoint)
        {
            rarity = _rarity;
            level = _level;
            basisPoint = _basisPoint;
        }

        public ItemData Generate()
        {
            GenerateItemData();
            return BakeItem();
        }

        protected abstract void GenerateItemData();

        private ItemData BakeItem()
        {
            return new ItemData(type, rarity, level, requiredChange, 
                _abilities.Values.ToList(), _properties.Values.ToList());
        }



    }
}
