using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Shop;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace GameDataEditor.ViewModels.Editor.ShopItems;
internal class ShopPriceViewModel : ViewModelBase
{
    private ShopItemBase _shopItem;
    private ShopPriceType _shopPriceType;
    private ShopResourcePrice? _resourcePrice;
    private ShopCurrentcyPrice? _currencyPrice;
    
    public ObservableCollection<ShopPriceType> priceTypes { get; } = new(Enum.GetValues<ShopPriceType>());
    public ObservableCollection<ResourceId> resourceIds { get; } = new(Enum.GetValues<ResourceId>());
    public ShopPriceType selectedPriceType
    {
        get => _shopPriceType;
        set
        {
            this.RaiseAndSetIfChanged(ref _shopPriceType, value);
            OnChangePriceType(value);
        }
    }
    public ShopResourcePrice? resourcePrice
    {
        get => _resourcePrice;
        set => this.RaiseAndSetIfChanged(ref _resourcePrice, value);
    }
    public ShopCurrentcyPrice? currencyPrice
    {
        get => _currencyPrice;
        set => this.RaiseAndSetIfChanged(ref _currencyPrice, value);
    }

    public ShopPriceViewModel(ShopItemBase shopItem)
    {
        _shopItem = shopItem;
        _shopPriceType = _shopItem.price?.priceType ?? ShopPriceType.Free;
        switch (shopItem.price)
        {
            case ShopResourcePrice resourcePrice:
                this.resourcePrice = resourcePrice;
                break;
            case ShopCurrentcyPrice currencyPrice:
                this.currencyPrice = currencyPrice;
                break;
        }
    }

    private void OnChangePriceType(ShopPriceType priceType)
    {
        switch (priceType)
        {
            case ShopPriceType.Free:
                _shopItem.price = null;
                resourcePrice = null;
                currencyPrice = null;
                break;
            case ShopPriceType.ResourcePrice:
                currencyPrice = null;
                resourcePrice = new();
                _shopItem.price = resourcePrice;
                break;
            case ShopPriceType.CurrencyPrice:
                resourcePrice = null;
                currencyPrice = new();
                _shopItem.price = currencyPrice;
                break;
        }
    }

}
