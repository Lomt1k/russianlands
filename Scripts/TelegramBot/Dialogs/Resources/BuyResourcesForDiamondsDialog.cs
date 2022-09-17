using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Resources
{
    public class BuyResourcesForDiamondsDialog : DialogBase
    {
        private Dictionary<ResourceType, int> _targetResources;
        private int _priceInDiamonds;
        private Func<Task> _onSuccess;
        private Func<Task> _onCancel;

        public BuyResourcesForDiamondsDialog(GameSession _session, Dictionary<ResourceType,int> targetResources, Func<Task> onSuccess, Func<Task> onCancel) : base(_session)
        {
            _targetResources = targetResources;
            _onSuccess = onSuccess;
            _onCancel = onCancel;

            foreach (var resource in _targetResources)
            {
                _priceInDiamonds += ResourceHelper.CalculatePriceInDiamonds(resource.Key, resource.Value);
            }
        }

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
            RegisterButton($"{Emojis.resources[ResourceType.Diamond]} {_priceInDiamonds}", () => TryPurchase());
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_button")}", _onCancel);

            await messageSender.SendTextDialog(session.chatId, sb.ToString(), GetMultilineKeyboard());
        }

        private async Task TryPurchase()
        {
            var playerResources = session.player.resources;
            bool success = playerResources.TryPurchase(ResourceType.Diamond, _priceInDiamonds);
            if (success)
            {
                playerResources.ForceAdd(_targetResources);
                await messageSender.SendTextMessage(session.chatId, Localization.Get(session, "resource_successfull_purshase_for_diamonds"));
                await _onSuccess();
                return;
            }

            ClearButtons();
            var text = string.Format(Localization.Get(session, "resource_not_enough_diamonds"), Emojis.smiles[Smile.Sad]);
            RegisterButton($"{Emojis.menuItems[MenuItem.Shop]} {Localization.Get(session, "menu_item_shop")}", null); // TODO
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_button")}", _onCancel);

            await messageSender.SendTextDialog(session.chatId, text, GetMultilineKeyboard());
        }

    }
}
