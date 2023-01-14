using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.Bot.Dialogs.Battle;
using TextGameRPG.Scripts.Bot.Dialogs.Resources;
using TextGameRPG.Scripts.GameCore.Managers;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap
{
    public class MobBattlePointData
    {
        public Mob mob;
        public int foodPrice;
        public List<RewardBase>? rewards;
        public Func<Task>? onBackButtonFunc;
        public Func<Player, BattleResult, Task>? onBattleEndFunc;
        public Func<Player, BattleResult, Task>? onContinueButtonFunc;
        public Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc;
    }

    public class MobBattlePointDialog : DialogBase
    {
        private MobBattlePointData _data;

        public MobBattlePointDialog(GameSession session, MobBattlePointData data) : base(session)
        {
            _data = data;
        }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.Append(Emojis.ButtonBattle + _data.mob.GetFullUnitInfoView(session));

            ClearButtons();
            var priceView = _data.foodPrice > 0 ? ResourceType.Food.GetEmoji() + _data.foodPrice.View() : string.Empty;
            var startBattleButton = Localization.Get(session, "dialog_mob_battle_point_start_battle", priceView);
            RegisterButton(startBattleButton, () => TryStartBattle());
            if (_data.onBackButtonFunc != null)
            {
                RegisterBackButton(_data.onBackButtonFunc);
            }

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

        public async Task SilentStart()
        {
            await TryStartBattle()
                .ConfigureAwait(false);
        }

        private async Task TryStartBattle()
        {
            var playerResources = session.player.resources;
            var successsPurchase = playerResources.TryPurchase(ResourceType.Food, _data.foodPrice);
            if (!successsPurchase)
            {
                var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, ResourceType.Food, _data.foodPrice,
                onSuccess: async () => await new MobBattlePointDialog(session, _data).SilentStart(),
                onCancel: async () => await new MobBattlePointDialog(session, _data).Start());
                await buyResourcesDialog.Start();
                return;
            }

            GlobalManagers.battleManager?.StartBattleWithMob(session.player, 
                _data.mob, _data.rewards, _data.onBattleEndFunc, _data.onContinueButtonFunc, _data.isAvailableReturnToTownFunc);
        }

    }
}
