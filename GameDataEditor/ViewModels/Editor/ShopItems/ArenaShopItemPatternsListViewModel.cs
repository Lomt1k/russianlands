using Avalonia.Controls;
using GameDataEditor.ViewModels.UserControls;
using GameDataEditor.Views.Editor.ShopItems;
using MarkOne.Scripts.GameCore.Arena;
using System.Threading.Tasks;

namespace GameDataEditor.ViewModels.Editor.ShopItems;
internal class ArenaShopItemPatternsListViewModel : EditorListViewModel<ArenaShopItemPattern>
{
    protected override Task<ArenaShopItemPattern?> CreateNewListItem()
    {
        var itemPattern = new ArenaShopItemPattern();
        return Task.FromResult(itemPattern);
    }

    protected override UserControl CreateViewForItem(ArenaShopItemPattern item)
    {
        return new ArenaShopItemPatternView() { DataContext = new ArenaShopItemPatternViewModel(item) };
    }
}
