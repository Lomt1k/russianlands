using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Dialogs.Resources;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map;

public class BattlePointData
{
    public Mob mob;
    public ResourceData price;
    public IEnumerable<RewardBase>? rewards;
    public Func<Task>? onBackButtonFunc;
    public Func<Player, BattleResult, Task>? onBattleEndFunc;
    public Func<Player, BattleResult, Task>? onContinueButtonFunc;
    public Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc;
}

public class BattlePointDialog : DialogBase
{
    protected readonly BattlePointData _data;
    protected readonly BattleManager _battleManager = Services.ServiceLocator.Get<BattleManager>();

    public BattlePointDialog(GameSession session, BattlePointData data) : base(session)
    {
        _data = data;
    }

    public override async Task Start()
    {
        var text = Emojis.ButtonBattle + _data.mob.GetFullUnitInfoView(session);

        ClearButtons();
        var priceView = _data.price.amount > 0 ? _data.price.resourceId.GetEmoji() + _data.price.amount.View() : string.Empty;
        var startBattleButton = Localization.Get(session, "dialog_mob_battle_point_start_battle", priceView);
        RegisterButton(startBattleButton, TryStartBattle);
        if (_data.onBackButtonFunc != null)
        {
            RegisterBackButton(_data.onBackButtonFunc);
        }

        await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
    }

    public async Task SilentStart()
    {
        await TryStartBattle().FastAwait();
    }

    protected virtual async Task TryStartBattle()
    {
        var playerResources = session.player.resources;
        var successsPurchase = playerResources.TryPurchase(_data.price);
        if (!successsPurchase)
        {
            var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, _data.price,
            onSuccess: async () => await new BattlePointDialog(session, _data).SilentStart(),
            onCancel: async () => await new BattlePointDialog(session, _data).Start());
            await buyResourcesDialog.Start().FastAwait();
            return;
        }

        _battleManager.StartBattleWithMob(session.player, _data.mob, _data.rewards,
            _data.onBattleEndFunc, _data.onContinueButtonFunc, _data.isAvailableReturnToTownFunc);
    }

}
