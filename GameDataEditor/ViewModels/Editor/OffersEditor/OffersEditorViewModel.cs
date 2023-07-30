using Avalonia.Controls;
using GameDataEditor.Models.RegularDialogs;
using GameDataEditor.Views.Editor.OffersEditor;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Shop.Offers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace GameDataEditor.ViewModels.Editor.OffersEditor;
public class OffersEditorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataBase = ServiceLocator.Get<GameDataHolder>();

    private OfferData? _selectedOffer;
    private UserControl? _offerView;

    public ObservableCollection<OfferData> showedOffers { get; private set; } = new();

    public OfferData? selectedOffer
    {
        get => _selectedOffer;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedOffer, value);
            offerView = CreateOfferView(value);
        }
    }

    public UserControl? offerView
    {
        get => _offerView;
        set => this.RaiseAndSetIfChanged(ref _offerView, value);
    }

    public ReactiveCommand<Unit, Unit> addNewOfferCommand { get; }
    public ReactiveCommand<Unit, Unit> removeOfferCommand { get; }

    public OffersEditorViewModel()
    {
        addNewOfferCommand = ReactiveCommand.Create(AddNewOffer);
        removeOfferCommand = ReactiveCommand.Create(RemoveSelectedOffer);

        RefreshShowedItems();
        gameDataBase.offers.onDataChanged += OnDataBaseChanged;
    }

    private void OnDataBaseChanged()
    {
        RefreshShowedItems();
    }

    private void RefreshShowedItems()
    {
        var offers = gameDataBase.offers.GetAllData();
        RefreshShowedItems(offers);
    }

    private void RefreshShowedItems(IEnumerable<OfferData> offers)
    {
        var oldSelectedId = selectedOffer?.id;
        showedOffers.Clear();

        foreach (var offer in offers)
        {
            showedOffers.Add(offer);
            if (offer.id == oldSelectedId)
            {
                selectedOffer = offer;
            }
        }
    }

    private UserControl CreateOfferView(OfferData offerData)
    {
        return offerData switch
        {
            PremiumOfferData offer => new OfferView() { DataContext = new PremiumOfferViewModel(offer) },
            OfferWithRewardsData offer => new OfferView() { DataContext = new OfferWithRewardsViewModel(offer) },
        };
    }

    private void AddNewOffer()
    {
        var allOffers = gameDataBase.offers.GetAllData().ToList();
        var newOfferId = allOffers.Count > 0 ? allOffers.Max(x => x.id) + 1 : 1;

        RegularDialogHelper.ShowItemSelectionDialog("Select offer type", new Dictionary<string, Action>()
        {
            { "Premium", () => AddNewOfferData(new PremiumOfferData(newOfferId)) },
            { "With Rewards", () => AddNewOfferData(new OfferWithRewardsData(newOfferId)) },
        });
    }

    private void AddNewOfferData(OfferData newOfferData)
    {
        gameDataBase.offers.AddData(newOfferData.id, newOfferData);
        RefreshShowedItems();
        selectedOffer = newOfferData;
    }

    private void RemoveSelectedOffer()
    {
        if (selectedOffer == null)
            return;

        gameDataBase.offers.RemoveData(selectedOffer.id);
    }
}
