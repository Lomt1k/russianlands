using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Items;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace MarkOne.ViewModels.Editor.ShopItems;
public class ArenaShopItemPatternViewModel : ViewModelBase
{
    private ArenaShopItemPattern _itemPattern;

    public ArenaShopItemPattern itemPattern
    {
        get => _itemPattern;
        set => this.RaiseAndSetIfChanged(ref _itemPattern, value);
    }
    public ObservableCollection<Rarity> allRarities { get; } = new ObservableCollection<Rarity>(Enum.GetValues<Rarity>());

    public ArenaShopItemPatternViewModel(ArenaShopItemPattern itemPattern)
    {
        _itemPattern = itemPattern;
    }
}
