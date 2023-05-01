using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Shop;

namespace TextGameRPG.Scripts.Bot.Dialogs.Resources
{
    public class BuyResourcesForDiamondsDialog : DialogBase
    {
        private Dictionary<ResourceId, int> _targetResources;
        private int _priceInDiamonds;
        private Func<Task> _onSuccess;
        private Func<Task> _onCancel;

        public BuyResourcesForDiamondsDialog(GameSession _session, Dictionary<ResourceId,int> targetResources, Func<Task> onSuccess, Func<Task> onCancel) : base(_session)
        {
            _targetResources = targetResources;
            _onSuccess = onSuccess;
            _onCancel = onCancel;

            foreach (var resource in _targetResources)
            {
                _priceInDiamonds += ResourceHelper.CalculatePriceInDiamonds(resource.Key, resource.Value);
            }
        }

        public BuyResourcesForDiamondsDialog(GameSession _session, ResourceId resourceId, int amount, Func<Task> onSuccess, Func<Task> onCancel)
            : this(_session, new Dictionary<ResourceId, int> { { resourceId, amount } }, onSuccess, onCancel) { }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "resource_not_enough"));

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_resources"));
            foreach (var resource in _targetResources)
            {
                sb.AppendLine(resource.Key.GetLocalizedView(session, resource.Value));
            }

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_purchase_for_diamonds"));
            RegisterButton(ResourceId.Diamond.GetEmoji() + _priceInDiamonds.ToString(), () => TryPurchase());
            RegisterBackButton(_onCancel);

            await SendDialogMessage(sb, GetMultilineKeyboard()).FastAwait();
        }

        private async Task TryPurchase()
        {
            var playerResources = session.player.resources;
            bool success = playerResources.TryPurchase(ResourceId.Diamond, _priceInDiamonds);
            if (success)
            {
                playerResources.ForceAdd(_targetResources);
                var sb = new StringBuilder();
                sb.AppendLine(Localization.Get(session, "resource_successfull_purshase_for_diamonds"));
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "resource_header_resources"));
                foreach (var resource in _targetResources)
                {
                    sb.AppendLine(resource.Key.GetLocalizedView(session, resource.Value));
                }

                await messageSender.SendTextMessage(session.chatId, sb.ToString()).FastAwait();
                await _onSuccess().FastAwait();
                return;
            }

            ClearButtons();
            var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
            RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"), 
                () => new ShopDialog(session).Start());
            RegisterBackButton(_onCancel);

            await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
        }

    }
}
