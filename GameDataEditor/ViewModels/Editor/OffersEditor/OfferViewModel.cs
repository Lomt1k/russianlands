using MarkOne.Scripts.GameCore.Shop.Offers;

namespace GameDataEditor.ViewModels.Editor.OffersEditor;
public abstract class OfferViewModel : ViewModelBase
{
    public OfferData offer { get; set; }

    public OfferViewModel(OfferData offerData)
    {
        offer = offerData;
    }
}
