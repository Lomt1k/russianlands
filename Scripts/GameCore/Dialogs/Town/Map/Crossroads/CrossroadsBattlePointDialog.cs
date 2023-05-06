using MarkOne.Scripts.GameCore.Dialogs.Resources;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map.Crossroads;

public class CrossroadsBattlePointDialog : BattlePointDialog
{
    public CrossroadsBattlePointDialog(GameSession session, BattlePointData data) : base(session, data)
    {
    }

    protected override async Task TryStartBattle()
    {
        ResourceHelper.RefreshCrossroadsEnergy(session);

        var playerResources = session.player.resources;
        var successsPurchase = playerResources.TryPurchase(_data.price);
        if (!successsPurchase)
        {
            var buyEnergyDialog = new BuyCrossroadsEnergyForDimondsDialog(session,
            onSuccess: async () => await new CrossroadsBattlePointDialog(session, _data).SilentStart(),
            onCancel: async () => await new CrossroadsBattlePointDialog(session, _data).Start());
            await buyEnergyDialog.Start().FastAwait();
            return;
        }

        _battleManager.StartBattle(session.player, _data.mob, _data.rewards,
            _data.onBattleEndFunc, _data.onContinueButtonFunc, _data.isAvailableReturnToTownFunc);
    }
}
