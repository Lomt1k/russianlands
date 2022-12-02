using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators
{
    public abstract class ItemCodeGeneratorBase
    {
        private List<AbilityType> _abilities = new List<AbilityType>();
        private List<PropertyType> _properties = new List<PropertyType>();

        protected ItemType type { get; private set; }
        protected Rarity rarity { get; private set; }
        protected byte townHallLevel { get; private set; }
        protected byte grade { get; private set; }
        protected StringBuilder sb { get; private set; }
        protected int abilitiesCount => _abilities.Count;
        protected int propertiesCount => _properties.Count;

        public ItemCodeGeneratorBase(ItemType _type, Rarity _rarity, int _townHallLevel)
        {
            type = _type;
            rarity = _rarity;
            townHallLevel = (byte)_townHallLevel;
            grade = (byte)ItemGenerationHelper.GetRandomGrade(townHallLevel);

            sb = new StringBuilder();

            var typeCode = type == ItemType.Boots ? "BT" : type.ToString().ToUpper().Substring(0, 2);
            sb.Append(typeCode);
            sb.Append(townHallLevel);
            sb.Append($"G{grade}");
            sb.Append($"R{(byte)rarity}");

            AppendSpecificInfo();
        }

        public abstract void AppendSpecificInfo();

        /// <summary>
        /// Добавляет предмету способность (всегда true)
        /// </summary>
        protected bool ForceAppendAbility(AbilityType abilityType)
        {
            _abilities.Add(abilityType);
            sb.Append($"A{(byte)abilityType}");
            return true;
        }

        /// <summary>
        /// Добавляет предмету свойство (всегда true)
        /// </summary>
        protected bool ForceAppendProperty(PropertyType propertyType)
        {
            _properties.Add(propertyType);
            sb.Append($"P{(byte)propertyType}");
            return true;
        }

        /// <summary>
        /// Добавит предмету способность, если такой способности еще не было
        /// </summary>
        protected bool AppendAbilityAsKeyword(AbilityType abilityType)
        {
            if (_abilities.Contains(abilityType))
                return false;

            _abilities.Add(abilityType);
            sb.Append($"A{(byte)abilityType}");
            return true;
        }

        /// <summary>
        /// Добавит предмету свойство, если такой способности еще не было
        /// </summary>
        protected bool AppendAbilityOnlyOnce(PropertyType propertyType)
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
