using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;
using System.Linq;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop.Offers;
public class OffersManager : Service
{
    private static readonly GameDataHolder gameDataBase = ServiceLocator.Get<GameDataHolder>();

    public async Task<OfferItem?> TryStartNextOffer(GameSession session)
    {
        var allOffersArray = gameDataBase.offersOrderedByPriority;
        foreach (var offerData in allOffersArray)
        {
            if (!offerData.isEnabled)
            {
                continue;
            }

            var offerItem = await TryStartOfferWithData(session, offerData).FastAwait();
            if (offerItem is not null)
            {
                return offerItem;
            }
        }
        return null;
    }

    private async Task<OfferItem?> TryStartOfferWithData(GameSession session, OfferData data)
    {
        var playerOffers = session.profile.dynamicData.offers;
        var offerItem = playerOffers.Where(x => x.id == data.id).FirstOrDefault();
        if (offerItem is null)
        {
            offerItem = new OfferItem(data.id);
            playerOffers.Add(offerItem);
        }

        if (!offerItem.IsCooldownRequired() && !offerItem.IsActivationsLimitReached())
        {
            var success = await offerItem.TryActivate(session).FastAwait();
            return success ? offerItem : null;
        }

        return null;
    }

}
