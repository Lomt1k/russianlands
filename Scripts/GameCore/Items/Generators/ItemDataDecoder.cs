using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators;

public struct ItemDataSeed
{
    public ItemType itemType;
    public Rarity rarity;
    public byte townHallLevel;
    public byte grade;
    public sbyte manaCost;

    public List<AbilityType> abilities;
    public List<PropertyType> properties;
    public List<string> baseParameters;

    public byte requiredLevel => (byte)ItemGenerationHelper.CalculateRequiredLevel(townHallLevel, grade);
}

public class ItemDataDecoder
{
    private ItemType _itemType;
    private Rarity _rarity;
    private byte _townHallLevel;
    private byte _grade;
    private sbyte _manaCost;

    private readonly List<AbilityType> _abilities = new List<AbilityType>();
    private readonly List<PropertyType> _properties = new List<PropertyType>();
    private readonly List<string> _baseParameters = new List<string>();

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
            ReadNextDigits(reader).TryParse(out _townHallLevel);
            ReadParameters(reader);
        }

        return new ItemDataSeed()
        {
            itemType = _itemType,
            townHallLevel = _townHallLevel,
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
        var typeArr = new char[2];
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
                var parameter = new string(new[] { firstChar, secondChar });
                _baseParameters.Add(parameter);
                return;
        }
    }



}
