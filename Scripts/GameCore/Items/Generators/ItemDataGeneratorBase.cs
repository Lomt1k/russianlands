using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    using ItemAbilities;
    using ItemProperties;
    using System;
    using System.Linq;

    public abstract partial class ItemDataGeneratorBase
    {
        protected ItemDataSeed seed { get; }
        protected byte requiredCharge { get; set; }
        protected float gradedPoints { get; }
        protected float gradeMult { get; }

        protected Dictionary<AbilityType, ItemAbilityBase> _abilities = new Dictionary<AbilityType, ItemAbilityBase>();
        protected Dictionary<PropertyType, ItemPropertyBase> _properties = new Dictionary<PropertyType, ItemPropertyBase>();

        public ItemDataGeneratorBase(ItemDataSeed _seed)
        {
            seed = _seed;
            gradeMult = (_seed.grade - 5) / 40f + 1; //от 0.9 до 1.125 (сам grade от 1 - 10)
            gradedPoints = _seed.basisPoints * gradeMult;
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

        protected void AddProperties()
        {
            foreach (var propertyType in seed.properties)
            {
                AddProperty(propertyType);
            }
        }

        protected void AddAbilities()
        {
            foreach (var abilityType in seed.abilities)
            {
                AddAbility(abilityType);
            }
        }

        //--- overriden for rings and amulets
        protected virtual void AddProperty(PropertyType propertyType)
        {
            switch (propertyType)
            {
                case PropertyType.IncreaseAttributeStrength:
                    var strength = (int)Math.Round(seed.requiredLevel * gradeMult / 10) + 1;
                    AddIncreaseAttributeStrength(strength);
                    break;
                case PropertyType.IncreaseAttributeVitality:
                    var vitality = (int)Math.Round(seed.requiredLevel * gradeMult / 10) + 1;
                    AddIncreaseAttributeVitality(vitality);
                    break;
                case PropertyType.IncreaseAttributeSorcery:
                    var sorcery = (int)Math.Round(seed.requiredLevel * gradeMult / 10) + 1;
                    AddIncreaseAttributeSorcery(sorcery);
                    break;
                case PropertyType.IncreaseAttributeLuck:
                    var luck = (int)Math.Round(seed.requiredLevel * gradeMult / 10) + 1;
                    AddIncreaseAttributeLuck(luck);
                    break;
                case PropertyType.IncreaseMaxHealth:
                    AddIncreaseMaxHealth((int)Math.Round(gradedPoints * 0.5));
                    break;
            }
        }

        protected virtual void AddAbility(AbilityType abilityType)
        {
        }



    }
}
