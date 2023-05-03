using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkOne.Scripts.GameCore.Resources;

public static partial class ResourceHelper
{
    private static readonly Dictionary<int, int> priceInDiamondsByResourceAmount = new Dictionary<int, int>
    {
        { 100, 1 },
        { 1_000, 5 },
        { 10_000, 25 },
        { 100_000, 125 },
        { 1_000_000, 600 },
        { 10_000_000, 3_000 },
        { 100_000_000, 15_000 },
    };

    private static readonly Dictionary<int, int> boostConstructionInDiamondsBySeconds = new Dictionary<int, int>
    {
        { 60, 1 },
        { 3_600, 35 },
        { 86_400, 490 },
        { 604_800, 1_950 },
    };

    private static readonly Dictionary<int, int> boostCraftInDiamondsBySeconds = new Dictionary<int, int>
    {
        { 60, 1 },
        { 43_200, 95 },
        { 86_400, 190 },
    };

    private static readonly int boostPotionCraftInDiamondsPerHour = 45;

    private static readonly KeyValuePair<int, int> minPriceInDiamondsByResource;
    private static readonly KeyValuePair<int, int> maxPriceInDiamondsByResource;
    private static readonly KeyValuePair<int, int> minBoostConstructionInDiamondsBySeconds;
    private static readonly KeyValuePair<int, int> maxBoostConstructionInDiamondsBySeconds;
    private static readonly KeyValuePair<int, int> minBoostCraftInDiamondsBySeconds;
    private static readonly KeyValuePair<int, int> maxBoostCraftInDiamondsBySeconds;

    public static Dictionary<ResourceId, float> resourceByDiamondsCoefs = new Dictionary<ResourceId, float>
    {
        { ResourceId.Diamond, 1 },
        { ResourceId.Food, 0.6f },
        { ResourceId.Gold, 0.5f },
        { ResourceId.Herbs, 1.75f },
        { ResourceId.Wood, 0.75f },
    };

    static ResourceHelper()
    {
        minPriceInDiamondsByResource = priceInDiamondsByResourceAmount.First();
        maxPriceInDiamondsByResource = priceInDiamondsByResourceAmount.Last();
        minBoostConstructionInDiamondsBySeconds = boostConstructionInDiamondsBySeconds.First();
        maxBoostConstructionInDiamondsBySeconds = boostConstructionInDiamondsBySeconds.Last();
        minBoostCraftInDiamondsBySeconds = boostCraftInDiamondsBySeconds.First();
        maxBoostCraftInDiamondsBySeconds = boostCraftInDiamondsBySeconds.Last();

        //ResourceHelper_Training.cs
        minBoostTrainingInDiamondsBySeconds = boostTrainingInDiamondsBySeconds.First();
        maxBoostTrainingInDiamondsBySeconds = boostTrainingInDiamondsBySeconds.Last();

        //ResourceHelper_CrossroadsEnergy.cs
        minEnergyPriceInDiamondsBySeconds = energyPriceInDiamondsBySeconds.First();
        maxEnergyPriceInDiamondsBySeconds = energyPriceInDiamondsBySeconds.Last();
    }

    public static int CalculatePriceInDiamonds(ResourceData resourceData)
    {
        var standardPrice = CalculateStandardPrice(resourceData.amount);
        var coef = resourceByDiamondsCoefs[resourceData.resourceId];

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

    public static ResourceData CalculateConstructionBoostPriceInDiamonds(int seconds)
    {
        int amount;
        if (seconds <= minBoostConstructionInDiamondsBySeconds.Key)
        {
            amount = minBoostConstructionInDiamondsBySeconds.Value;
            return new ResourceData(ResourceId.Diamond, amount);
        }

        if (seconds >= maxBoostConstructionInDiamondsBySeconds.Key)
        {
            var mult = (float)seconds / maxBoostConstructionInDiamondsBySeconds.Key;
            amount = (int)(maxBoostConstructionInDiamondsBySeconds.Value * mult);
            return new ResourceData(ResourceId.Diamond, amount);
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

        amount = (int)Math.Round(progression * priceDelta) + lowerKVP.Value;
        return new ResourceData(ResourceId.Diamond, amount);
    }

    public static ResourceData CalculateCraftBoostPriceInDiamonds(int seconds)
    {
        int amount;
        if (seconds <= minBoostCraftInDiamondsBySeconds.Key)
        {
            amount = minBoostCraftInDiamondsBySeconds.Value;
            return new ResourceData(ResourceId.Diamond, amount);
        }

        if (seconds >= maxBoostCraftInDiamondsBySeconds.Key)
        {
            var mult = (float)seconds / maxBoostCraftInDiamondsBySeconds.Key;
            amount = (int)(maxBoostCraftInDiamondsBySeconds.Value * mult);
            return new ResourceData(ResourceId.Diamond, amount);
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

        amount = (int)Math.Round(progression * priceDelta) + lowerKVP.Value;
        return new ResourceData(ResourceId.Diamond, amount);
    }

    public static ResourceData CalculatePotionCraftBoostPriceInDiamonds(int seconds)
    {
        var hours = (float)seconds / 3600;
        var price = (int)Math.Round(hours * boostPotionCraftInDiamondsPerHour);
        var amount = Math.Max(price, 1);
        return new ResourceData(ResourceId.Diamond, amount);
    }


}
