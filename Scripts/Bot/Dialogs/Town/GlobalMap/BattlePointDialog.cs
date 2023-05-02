using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Battle;
using TextGameRPG.Scripts.Bot.Dialogs.Resources;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.Battles;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap;

public class BattlePointData
{
    public Mob mob;
    public int foodPrice;
    public List<RewardBase>? rewards;
    public Func<Task>? onBackButtonFunc;
    public Func<Player, BattleResult, Task>? onBattleEndFunc;
    public Func<Player, BattleResult, Task>? onContinueButtonFunc;
    public Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc;
}

public class BattlePointDialog : DialogBase
{
    private readonly BattlePointData _data;
    private readonly BattleManager _battleManager = Services.Get<BattleManager>();

    public BattlePointDialog(GameSession session, BattlePointData data) : base(session)
    {
        _data = data;
    }

    public override async Task Start()
    {
        var text = Emojis.ButtonBattle + _data.mob.GetFullUnitInfoView(session);

        ClearButtons();
        var priceView = _data.foodPrice > 0 ? ResourceId.Food.GetEmoji() + _data.foodPrice.View() : string.Empty;
        var startBattleButton = Localization.Get(session, "dialog_mob_battle_point_start_battle", priceView);
        RegisterButton(startBattleButton, () => TryStartBattle());
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

    private async Task TryStartBattle()
    {
        var playerResources = session.player.resources;
        var foodPrice = new ResourceData(ResourceId.Food, _data.foodPrice);
        var successsPurchase = playerResources.TryPurchase(foodPrice);
        if (!successsPurchase)
        {
            var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, foodPrice,
            onSuccess: async () => await new BattlePointDialog(session, _data).SilentStart(),
            onCancel: async () => await new BattlePointDialog(session, _data).Start());
            await buyResourcesDialog.Start().FastAwait();
            return;
        }

        _battleManager.StartBattleWithMob(session.player, _data.mob, _data.rewards,
            _data.onBattleEndFunc, _data.onContinueButtonFunc, _data.isAvailableReturnToTownFunc);
    }

}
