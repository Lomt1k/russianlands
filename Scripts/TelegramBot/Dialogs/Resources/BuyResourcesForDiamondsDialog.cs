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
        private Func<Task> _onSuccess;
        private Func<Task> _onCancel;

        private static Dictionary<int, int> priceInDiamondsByResourceAmount = new Dictionary<int, int>
        {
            { 100, 1 },
            { 1_000, 5 },
            { 10_000, 25 },
            { 100_000, 125 },
            { 1_000_000, 600 },
            { 10_000_000, 3_000 },
            { 100_000_000, 15_000 },
        };

        public static Dictionary<ResourceType, float> resourceByDiamondsCoefs = new Dictionary<ResourceType, float>
        {
            { ResourceType.Diamond, 1 },
            { ResourceType.Food, 0.6f },
            { ResourceType.Gold, 0.5f },
            { ResourceType.Herbs, 1.75f },
            { ResourceType.Wood, 0.75f },
        };

        public BuyResourcesForDiamondsDialog(GameSession _session, Dictionary<ResourceType,int> targetResources, Func<Task> onSuccess, Func<Task> onCancel) : base(_session)
        {
            _targetResources = targetResources;
            _onSuccess = onSuccess;
            _onCancel = onCancel;
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

            int price = 0;
            foreach (var resource in _targetResources)
            {
                price += CalculatePriceInDiamonds(resource.Key, resource.Value);
            }

            var priceText = $"{Emojis.resources[ResourceType.Diamond]} {price}";
            RegisterButton(priceText, null);

            await messageSender.SendTextDialog(session.chatId, sb.ToString(), GetMultilineKeyboard());
        }

        private static int CalculatePriceInDiamonds(ResourceType resourceType, int resourceAmount)
        {
            var standardPrice = CalculateStandardPrice(resourceAmount);
            var coef = resourceByDiamondsCoefs[resourceType];

            var resultPrice = (int)(standardPrice * coef);
            return Math.Max(resultPrice, 1);
        }

        private static int CalculateStandardPrice(int resourceAmount)
        {
            if (resourceAmount < 100)
                return 1;
            if (resourceAmount < 1_000)
            {

            }
        }
    }
}
