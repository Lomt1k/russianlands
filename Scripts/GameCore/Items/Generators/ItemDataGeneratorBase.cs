using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    using ItemAbilities;
    using ItemProperties;
    using System.Linq;

    internal abstract partial class ItemDataGeneratorBase
    {
        protected ItemDataSeed seed { get; }
        protected byte requiredCharge { get; set; }

        private Dictionary<AbilityType, ItemAbilityBase> _abilities = new Dictionary<AbilityType, ItemAbilityBase>();
        private Dictionary<PropertyType, ItemPropertyBase> _properties = new Dictionary<PropertyType, ItemPropertyBase>();

        public ItemDataGeneratorBase(ItemDataSeed _seed)
        {
            seed = _seed;
        }

        public ItemData Generate()
        {
            GenerateItemData();
            return BakeItem();
        }

        protected abstract void GenerateItemData();

        private ItemData BakeItem()
        {
            return new ItemData(seed.itemType, seed.rarity, seed.requiredLevel, requiredCharge, 
                _abilities.Values.ToList(), _properties.Values.ToList());
        }



    }
}
