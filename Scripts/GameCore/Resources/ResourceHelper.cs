using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public static partial class ResourceHelper
    {
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

        private static Dictionary<int, int> boostConstructionInDiamondsBySeconds = new Dictionary<int, int>
        {
            { 60, 1 },
            { 3_600, 35 },
            { 86_400, 490 },
            { 604_800, 1_950 },
        };

        private static Dictionary<int, int> boostCraftInDiamondsBySeconds = new Dictionary<int, int>
        {
            { 60, 1 },
            { 43_200, 95 },
            { 86_400, 190 },
        };

        private static int boostPotionCraftInDiamondsPerHour = 45;

        private static KeyValuePair<int, int> minPriceInDiamondsByResource;
        private static KeyValuePair<int, int> maxPriceInDiamondsByResource;
        private static KeyValuePair<int, int> minBoostConstructionInDiamondsBySeconds;
        private static KeyValuePair<int, int> maxBoostConstructionInDiamondsBySeconds;
        private static KeyValuePair<int, int> minBoostCraftInDiamondsBySeconds;
        private static KeyValuePair<int, int> maxBoostCraftInDiamondsBySeconds;

        public static Dictionary<ResourceType, float> resourceByDiamondsCoefs = new Dictionary<ResourceType, float>
        {
            { ResourceType.Diamond, 1 },
            { ResourceType.Food, 0.6f },
            { ResourceType.Gold, 0.5f },
            { ResourceType.Herbs, 1.75f },
            { ResourceType.Wood, 0.75f },
        };

        static ResourceHelper()
        {
            minPriceInDiamondsByResource = priceInDiamondsByResourceAmount.First();
            maxPriceInDiamondsByResource = priceInDiamondsByResourceAmount.Last();
            minBoostConstructionInDiamondsBySeconds = boostConstructionInDiamondsBySeconds.First();
            maxBoostConstructionInDiamondsBySeconds = boostConstructionInDiamondsBySeconds.Last();
            minBoostCraftInDiamondsBySeconds = boostCraftInDiamondsBySeconds.First();
            maxBoostCraftInDiamondsBySeconds = boostCraftInDiamondsBySeconds.Last();

            //ResourceHelperTraining.cs
            minBoostTrainingInDiamondsBySeconds = boostTrainingInDiamondsBySeconds.First();
            maxBoostTrainingInDiamondsBySeconds = boostTrainingInDiamondsBySeconds.Last();
        }

        public static int CalculatePriceInDiamonds(ResourceType resourceType, int resourceAmount)
        {
            var standardPrice = CalculateStandardPrice(resourceAmount);
            var coef = resourceByDiamondsCoefs[resourceType];

            var resultPrice = (int)Math.Round(standardPrice * coef);
            return Math.Max(resultPrice, 1);
        }

        private static int CalculateStandardPrice(int resourceAmount)
        {
            if (resourceAmount <= minPriceInDiamondsByResource.Key)
                return minPriceInDiamondsByResource.Value;

            if (resourceAmount >= maxPriceInDiamondsByResource.Key)
            {
                var mult = (float)resourceAmount / maxPriceInDiamondsByResource.Key;
                return (int)(maxPriceInDiamondsByResource.Value * mult);
            }

            var lowerKVP = new KeyValuePair<int, int>();
            var upperKVP = new KeyValuePair<int, int>();
            foreach (var kvp in priceInDiamondsByResourceAmount)
            {
                if (resourceAmount < kvp.Key)
                {
                    upperKVP = kvp;
                    break;
                }
                lowerKVP = kvp;
            }

            var resourceDelta = upperKVP.Key - lowerKVP.Key;
            var priceDelta = upperKVP.Value - lowerKVP.Value;
            var progression = (float)(resourceAmount - lowerKVP.Key) / resourceDelta;

            return (int)Math.Round(progression * priceDelta) + lowerKVP.Value;
        }

        public static string GetResourcesView(GameSession session, Dictionary<ResourceType, int> resources)
        {
            var sb = new StringBuilder();
            foreach (var kvp in resources)
            {
                var resourceType = kvp.Key;
                var resourceAmount = kvp.Value;
                sb.AppendLine(resourceType.GetLocalizedView(session, resourceAmount));
            }
            return sb.ToString();
        }

        public static string GetPriceView(GameSession session, Dictionary<ResourceType,int> resources)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "resource_header_price"));

            bool hasAmount = false;
            foreach (var kvp in resources)
            {
                var resourceType = kvp.Key;
                var resourceAmount = kvp.Value;
                if (resourceAmount < 1)
                    continue;

                hasAmount = true;
                sb.AppendLine(resourceType.GetLocalizedView(session, resourceAmount));
            }

            if (!hasAmount)
            {
                sb.AppendLine(Localization.Get(session, "resource_price_free"));
            }

            return sb.ToString();
        }

        public static string GetCompactResourcesView(Dictionary<ResourceType, int> resources)
        {
            var sb = new StringBuilder();
            int elementsInCurrentRow = 0;
            foreach (var resource in resources)
            {
                if (elementsInCurrentRow == 3)
                {
                    sb.AppendLine();
                    elementsInCurrentRow = 0;
                }
                if (elementsInCurrentRow > 0)
                {
                    sb.Append(Emojis.bigSpace);
                }

                sb.Append(resource.Key.GetShortView(resource.Value));
                elementsInCurrentRow++;
            }

            return sb.ToString();
        }

        public static int CalculateConstructionBoostPriceInDiamonds(int seconds)
        {
            if (seconds <= minBoostConstructionInDiamondsBySeconds.Key)
                return minBoostConstructionInDiamondsBySeconds.Value;

            if (seconds >= maxBoostConstructionInDiamondsBySeconds.Key)
            {
                var mult = (float)seconds / maxBoostConstructionInDiamondsBySeconds.Key;
                return (int)(maxBoostConstructionInDiamondsBySeconds.Value * mult);
            }

            var lowerKVP = new KeyValuePair<int, int>();
            var upperKVP = new KeyValuePair<int, int>();
            foreach (var kvp in boostConstructionInDiamondsBySeconds)
            {
                if (seconds < kvp.Key)
                {
                    upperKVP = kvp;
                    break;
                }
                lowerKVP = kvp;
            }

            var secondsDelta = upperKVP.Key - lowerKVP.Key;
            var priceDelta = upperKVP.Value - lowerKVP.Value;
            var progression = (float)(seconds - lowerKVP.Key) / secondsDelta;

            return (int)Math.Round(progression * priceDelta) + lowerKVP.Value;
        }

        public static int CalculateCraftBoostPriceInDiamonds(int seconds)
        {
            if (seconds <= minBoostCraftInDiamondsBySeconds.Key)
                return minBoostCraftInDiamondsBySeconds.Value;

            if (seconds >= maxBoostCraftInDiamondsBySeconds.Key)
            {
                var mult = (float)seconds / maxBoostCraftInDiamondsBySeconds.Key;
                return (int)(maxBoostCraftInDiamondsBySeconds.Value * mult);
            }

            var lowerKVP = new KeyValuePair<int, int>();
            var upperKVP = new KeyValuePair<int, int>();
            foreach (var kvp in boostCraftInDiamondsBySeconds)
            {
                if (seconds < kvp.Key)
                {
                    upperKVP = kvp;
                    break;
                }
                lowerKVP = kvp;
            }

            var secondsDelta = upperKVP.Key - lowerKVP.Key;
            var priceDelta = upperKVP.Value - lowerKVP.Value;
            var progression = (float)(seconds - lowerKVP.Key) / secondsDelta;

            return (int)Math.Round(progression * priceDelta) + lowerKVP.Value;
        }

        public static int CalculatePotionCraftBoostPriceInDiamonds(int seconds)
        {
            var hours = (float)seconds / 3600;
            var price = (int)Math.Round(hours * boostPotionCraftInDiamondsPerHour);
            return Math.Max(price, 1);
        }


    }
}
