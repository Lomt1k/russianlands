using System;
using TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    /// <summary>
    /// Основной класс, который управляет логикой генерации предметов
    /// </summary>
    public static class ItemGenerationManager
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
            return GenerateItem(_type, _rarity, _settings.townHallLevel);
        }

        private static InventoryItem GenerateItem(ItemType _type, Rarity _rarity, int _townHallLevel)
        {
            string dataCode = _type switch
            {
                ItemType.Sword => new SwordCodeGenerator(_type, _rarity, _townHallLevel).GetCode(),
                ItemType.Bow => new BowCodeGenerator(_type, _rarity, _townHallLevel).GetCode(),
                ItemType.Stick => new StickCodeGenerator(_type, _rarity, _townHallLevel).GetCode(),
                ItemType.Armor => new ArmorCodeGenerator(_type, _rarity, _townHallLevel).GetCode(),
                ItemType.Boots => new ArmorCodeGenerator(_type, _rarity, _townHallLevel).GetCode(),
                ItemType.Helmet => new ArmorCodeGenerator(_type, _rarity, _townHallLevel).GetCode(),
                ItemType.Shield => new ShieldCodeGenerator(_type, _rarity, _townHallLevel).GetCode(),
                ItemType.Amulet => new AmuletCodeGenerator(_type, _rarity, _townHallLevel).GetCode(),
                ItemType.Ring => new RingCodeGenerator(_type, _rarity, _townHallLevel).GetCode(),
                ItemType.Scroll => new ScrollCodeGenerator(_type, _rarity, _townHallLevel).GetCode(),
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(dataCode))
            {
                Program.logger.Error($"ItemGenerationManager: Can not create item with type '{_type}'. Creating a sword...");
                dataCode = new SwordCodeGenerator(ItemType.Sword, _rarity, _townHallLevel).GetCode();
            }

            return new InventoryItem(dataCode);
        }


    }
}
