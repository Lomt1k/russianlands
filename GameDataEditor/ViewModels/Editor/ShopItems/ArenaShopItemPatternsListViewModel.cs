using Avalonia.Controls;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.ViewModels.UserControls;
using MarkOne.Views.Editor.ShopItems;
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
