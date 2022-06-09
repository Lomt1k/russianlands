using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    using ItemAbilities;
    using ItemProperties;
    using System;
    using System.Linq;

    internal abstract partial class ItemDataGeneratorBase
    {
        protected abstract ItemType type { get; }

        protected ItemRarity rarity { get; }
        protected ushort level { get; }
        protected int basisPoint { get; }
        protected byte requiredChange { get; set; }

        private List<ItemAbilityBase> _abilities = new List<ItemAbilityBase>();
        private List<ItemPropertyBase> _properties = new List<ItemPropertyBase>();

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
            var abilities = BakeAbilities();
            var properties = BakeProperties();
            return new ItemData(type, rarity, level, requiredChange, abilities, properties);
        }

        private List<ItemAbilityBase> BakeAbilities()
        {
            Dictionary<AbilityType, ItemAbilityBase> result = new Dictionary<AbilityType, ItemAbilityBase>();

            for (int i = 0; i < _abilities.Count; i++)
            {
                var ability = _abilities[i];
                if (result.ContainsKey(ability.abilityType))
                    continue;
                
                for (int j = i + 1; j < _abilities.Count; j++)
                {
                    var tempAbility = _abilities[j];
                    if (tempAbility.abilityType == ability.abilityType)
                    {
                        MergeFields(ability, tempAbility);
                    }
                }
                result.Add(ability.abilityType, ability);
            }

            return result.Values.ToList();
        }

        private List<ItemPropertyBase> BakeProperties()
        {
            Dictionary<PropertyType, ItemPropertyBase> result = new Dictionary<PropertyType, ItemPropertyBase>();

            for (int i = 0; i < _properties.Count; i++)
            {
                var property = _properties[i];
                if (result.ContainsKey(property.propertyType))
                    continue;

                for (int j = i + 1; j < _properties.Count; j++)
                {
                    var tempProperty = _properties[j];
                    if (tempProperty.propertyType == property.propertyType)
                    {
                        MergeFields(property, tempProperty);
                    }
                }
                result.Add(property.propertyType, property);
            }

            return result.Values.ToList();
        }

        private void MergeFields(object baseAbility, object additive)
        {
            var fields = baseAbility.GetType().GetFields();
            foreach (var field in fields)
            {
                switch (Type.GetTypeCode(field.FieldType))
                {
                    case TypeCode.Int16: field.SetValue(baseAbility, (short)field.GetValue(baseAbility) + (short)field.GetValue(additive)); break;
                    case TypeCode.Int32: field.SetValue(baseAbility, (int)field.GetValue(baseAbility) + (int)field.GetValue(additive)); break;
                    case TypeCode.Int64: field.SetValue(baseAbility, (long)field.GetValue(baseAbility) + (long)field.GetValue(additive)); break;
                    case TypeCode.UInt16: field.SetValue(baseAbility, (ushort)field.GetValue(baseAbility) + (ushort)field.GetValue(additive)); break;
                    case TypeCode.UInt32: field.SetValue(baseAbility, (uint)field.GetValue(baseAbility) + (uint)field.GetValue(additive)); break;
                    case TypeCode.UInt64: field.SetValue(baseAbility, (ulong)field.GetValue(baseAbility) + (ulong)field.GetValue(additive)); break;
                    case TypeCode.SByte: field.SetValue(baseAbility, (sbyte)field.GetValue(baseAbility) + (sbyte)field.GetValue(additive)); break;
                    case TypeCode.Byte: field.SetValue(baseAbility, (byte)field.GetValue(baseAbility) + (byte)field.GetValue(additive)); break;
                    case TypeCode.Decimal: field.SetValue(baseAbility, (decimal)field.GetValue(baseAbility) + (decimal)field.GetValue(additive)); break;
                    case TypeCode.Double: field.SetValue(baseAbility, (double)field.GetValue(baseAbility) + (double)field.GetValue(additive)); break;
                    case TypeCode.Single: field.SetValue(baseAbility, (float)field.GetValue(baseAbility) + (float)field.GetValue(additive)); break;
                }
            }
        }



    }
}
