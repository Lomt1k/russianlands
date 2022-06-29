using System;
using System.Collections.ObjectModel;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Models.Editor.ItemsEditor
{
    public class ItemCategory
    {
        public string name { get; }
        public ItemType itemType { get; }

        public ItemCategory(string _name, ItemType _type)
        {
            name = _name;
            itemType = _type;
        }

        public static ObservableCollection<ItemCategory> GetAllCategories()
        {
            var items = new ObservableCollection<ItemCategory>();
            foreach (var element in Enum.GetValues(typeof(ItemType)))
            {
                var name = element.ToString();
                var itemType = (ItemType)Enum.Parse(typeof(ItemType), name);
                if (itemType == ItemType.Equipped)
                    continue;

                var item = new ItemCategory(name, itemType);
                items.Add(item);
            }

            //Set 'Any' caterogy first
            var sortedItems = new ObservableCollection<ItemCategory>();
            sortedItems.Add(items[items.Count - 1]);
            for (int i = 0; i < items.Count - 1; i++)
            {
                sortedItems.Add(items[i]);
            }

            return sortedItems;
        }

    }
}
