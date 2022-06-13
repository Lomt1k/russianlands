using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    public struct ItemDataSeed
    {
        public ItemType itemType;
        public int basisPoints;
        public ushort requiredLevel;
        public ItemRarity rarity;
        public byte grade;
        public byte manaCost;

        public List<AbilityType> abilities;
        public List<PropertyType> properties;
        public List<string> baseParameters;
    }

    internal class ItemDataDecoder
    {
        private ItemType _itemType;
        private int _basisPoints;
        private ushort _requiredLevel;
        private ItemRarity _rarity;
        private byte _grade;
        private byte _manaCost;

        private List<AbilityType> _abilities = new List<AbilityType>();
        private List<PropertyType> _properties = new List<PropertyType>();
        private List<string> _baseParameters = new List<string>();

        public static ItemData Decode(string dataCode)
        {
            var decoder = new ItemDataDecoder();
            var seed = decoder.ParseSeedFromCode(dataCode);

            switch (seed.itemType)
            {
                case ItemType.Sword: return new SwordDataGenerator(seed).Generate();
                case ItemType.Bow: return new BowDataGenerator(seed).Generate();
                case ItemType.Stick: return new StickDataGenerator(seed).Generate();
                case ItemType.Armor: return new ArmorDataGenerator(seed).Generate();
                case ItemType.Helmet: return new HelmetDataGenerator(seed).Generate();
                case ItemType.Boots: return new BootsDataGenerator(seed).Generate();
                case ItemType.Shield: return new ShieldDataGenerator(seed).Generate();
                case ItemType.Amulet: return new AmuletDataGenerator(seed).Generate();
                case ItemType.Ring: return new RingDataGenerator(seed).Generate();
                case ItemType.Scroll: return new ScrollDataGenerator(seed).Generate();
            }
            return ItemData.brokenItem;
        }

        private ItemDataSeed ParseSeedFromCode(string dataCode)
        {
            using (var reader = new StringReader(dataCode))
            {
                _itemType = ReadType(reader);
                ReadNextDigits(reader).TryParse(out _basisPoints);
                ReadParameters(reader);
            }

            return new ItemDataSeed()
            {
                itemType = _itemType,
                basisPoints = _basisPoints,
                requiredLevel = _requiredLevel,
                rarity = _rarity,
                grade = _grade,
                abilities = _abilities,
                properties = _properties,
                baseParameters = _baseParameters,
                manaCost = _manaCost,
            };
        }

        private ItemType ReadType(StringReader reader)
        {
            char[] typeArr = new char[2];
            reader.ReadBlock(typeArr, 0, 2);
            var itemTypeCode = new string(typeArr);
            switch (itemTypeCode)
            {
                case "SW": return ItemType.Sword;
                case "BO": return ItemType.Bow;
                case "ST": return ItemType.Stick;
                case "HE": return ItemType.Helmet;
                case "AR": return ItemType.Armor;
                case "BT": return ItemType.Boots;
                case "SH": return ItemType.Shield;
                case "RI": return ItemType.Ring;
                case "AM": return ItemType.Amulet;
                case "SC": return ItemType.Scroll;
                case "PO": return ItemType.Poison;
            }
            throw new Exception($"Error when parsed item type with type code: {itemTypeCode}");
        }

        private string ReadNextDigits(StringReader reader)
        {
            var digits = new LinkedList<char>();
            while (reader.Peek() > -1)
            {
                var nextChar = (char)reader.Peek();
                if (!char.IsDigit(nextChar))
                    break;

                digits.AddLast(nextChar);
                reader.Read();
            }

            return new string(digits.ToArray());
        }

        private void ReadParameters(StringReader reader)
        {
            while (reader.Peek() > -1)
            {
                ReadNextParameter(reader);
            }
        }

        private void ReadNextParameter(StringReader reader)
        {
            var firstChar = (char)reader.Read();
            switch (firstChar)
            {
                case 'L':
                    ReadNextDigits(reader).TryParse(out _requiredLevel);
                    return;
                case 'R':
                    ReadNextDigits(reader).TryParse(out _rarity);
                    return;
                case 'G':
                    ReadNextDigits(reader).TryParse(out _grade);
                    return;
                case 'M':
                    ReadNextDigits(reader).TryParse(out _manaCost);
                    return;
                case 'A':
                    ReadNextDigits(reader).TryParse(out AbilityType abilityType);
                    _abilities.Add(abilityType);
                    return;
                case 'P':
                    ReadNextDigits(reader).TryParse(out PropertyType propertyType);
                    _properties.Add(propertyType);
                    return;

                default:
                    var secondChar = (char)reader.Read();
                    var parameter = new string( new[]{ firstChar, secondChar } );
                    _baseParameters.Add(parameter);
                    return;
            }
        }



    }
}
