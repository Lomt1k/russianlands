using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    internal abstract class ItemCodeGeneratorBase
    {
        protected ItemType type { get; private set; }
        protected Rarity rarity { get; private set; }
        protected ushort requiredLevel { get; private set; }
        protected int basisPoints { get; private set; }

        protected StringBuilder sb { get; private set; }

        private List<AbilityType> _abilities = new List<AbilityType>();
        private List<PropertyType> _properties = new List<PropertyType>();

        public ItemCodeGeneratorBase(ItemType _type, Rarity _rarity, ushort _level, int _basisPoints)
        {
            type = _type;
            rarity = _rarity;
            requiredLevel = _level;
            basisPoints = _basisPoints;
            sb = new StringBuilder();

            var typeCode = type == ItemType.Boots ? "BT" : type.ToString().ToUpper().Substring(0, 2);
            sb.Append(typeCode);
            sb.Append(basisPoints);
            sb.Append($"L{requiredLevel}");
            sb.Append($"R{(byte)rarity}");

            var grade = Randomizer.GetGrade();
            sb.Append($"G{grade}");

            AppendSpecificInfo();
        }

        public abstract void AppendSpecificInfo();

        protected bool TryAppendAbility(AbilityType abilityType)
        {
            if (_abilities.Contains(abilityType))
                return false;

            _abilities.Add(abilityType);
            sb.Append($"A{(byte)abilityType}");
            return true;
        }

        protected bool TryAppendProperty(PropertyType propertyType)
        {
            if (_properties.Contains(propertyType))
                return false;

            _properties.Add(propertyType);
            sb.Append($"P{(byte)propertyType}");
            return true;
        }

        public string GetCode()
        {
            return sb.ToString();
        }

    }
}
