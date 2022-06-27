using System;
using TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    /// <summary>
    /// Основной класс, который управляет логикой генерации предметов
    /// </summary>
    internal static class ItemGenerationManager
    {
        private static int _minItemType = (int)ItemType.Sword;
        private static int _maxItemType = (int)ItemType.Scroll + 1;

        public static InventoryItem GenerateItem(ItemGenerationSettings _settings, Rarity _rarity)
        {
            bool success = false;
            var itemType = ItemType.Any;
            while (!success)
            {
                itemType = (ItemType)new Random().Next(_minItemType, _maxItemType);
                success = _rarity != Rarity.Common || itemType.IsSupportCommonRarity();
            }            
            return GenerateItem(_settings, itemType, _rarity);
        }

        public static InventoryItem GenerateItem(ItemGenerationSettings _settings, ItemType _type, Rarity _rarity)
        {
            ushort level;
            switch (_rarity)
            {
                case Rarity.Common: level = _settings.levelForCommonItems; break;
                case Rarity.Rare: level = _settings.levelForRareItems; break;
                case Rarity.Epic: level = _settings.levelForEpicItems; break;
                case Rarity.Legendary: level = _settings.levelForLegendaryItems; break;
                default: level = _settings.levelForLegendaryItems; break;
            }
            return GenerateItem(_type, _rarity, level, _settings.basisPoints);
        }

        private static InventoryItem GenerateItem(ItemType _type, Rarity _rarity, ushort _level, int _basisPoints)
        {
            string dataCode;
            switch (_type)
            {
                case ItemType.Sword:
                case ItemType.Bow:
                    dataCode = new WeaponCodeGenerator(_type, _rarity, _level, _basisPoints).GetCode();
                    break;
                case ItemType.Stick:
                    dataCode = new StickCodeGenerator(_type, _rarity, _level, _basisPoints).GetCode();
                    break;
                case ItemType.Armor:
                case ItemType.Boots:
                case ItemType.Helmet:
                    dataCode = new ArmorCodeGenerator(_type, _rarity, _level, _basisPoints).GetCode();
                    break;
                case ItemType.Shield:
                    dataCode = new ShieldCodeGenerator(_type, _rarity, _level, _basisPoints).GetCode();
                    break;
                case ItemType.Amulet:
                    dataCode = new AmuletCodeGenerator(_type, _rarity, _level, _basisPoints).GetCode();
                    break;
                case ItemType.Ring:
                    dataCode = new RingCodeGenerator(_type, _rarity, _level, _basisPoints).GetCode();
                    break;
                case ItemType.Scroll:
                    dataCode = new ScrollCodeGenerator(_type, _rarity, _level, _basisPoints).GetCode();
                    break;

                default: //unsupported ItemType
                    Program.logger.Error($"ItemGenerationManager: Can not create item with type {_type}. Creating a sword...");
                    dataCode = new WeaponCodeGenerator(ItemType.Sword, _rarity, _level, _basisPoints).GetCode();
                    break;
            }

            return new InventoryItem(dataCode);
        }


    }
}
