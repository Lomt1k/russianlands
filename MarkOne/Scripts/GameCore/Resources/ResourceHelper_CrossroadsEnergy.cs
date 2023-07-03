using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Resources;
public static partial class ResourceHelper
{
    private const int SECONDS_FOR_ENERGY = 7200;

    public static (ResourceData resourceData, int resourceLimit, TimeSpan timeUntilNextEnergy) RefreshCrossroadsEnergy(GameSession session)
    {
        var playerResources = session.player.resources;
        var now = DateTime.UtcNow;
        var lastUpdateTime = session.profile.data.lastCrossroadsResourceUpdate;
        var nextTime = lastUpdateTime;
        int energyToAdd = 0;
        while (nextTime <= now)
        {
            nextTime = lastUpdateTime.AddSeconds(SECONDS_FOR_ENERGY);
            if (nextTime <= now)
            {
                energyToAdd++;
                lastUpdateTime = nextTime;
            }
        }
        session.profile.data.lastCrossroadsResourceUpdate = lastUpdateTime;
        playerResources.Add(new ResourceData(ResourceId.CrossroadsEnergy, energyToAdd));
        var resourceData = playerResources.GetResourceData(ResourceId.CrossroadsEnergy);
        var resourceLimit = playerResources.GetResourceLimit(ResourceId.CrossroadsEnergy);
        return (resourceData, resourceLimit, nextTime - now);
    }

    private static readonly Dictionary<int, int> energyPriceInDiamondsBySeconds = new Dictionary<int, int>
    {
        { 60, 1 },
        { 300, 2 },
        { 600, 3 },
        { 1200, 6 },
        { 7200, 29 },
    };

    private static readonly KeyValuePair<int, int> minEnergyPriceInDiamondsBySeconds;
    private static readonly KeyValuePair<int, int> maxEnergyPriceInDiamondsBySeconds;

    public static ResourceData CalculateCrossroadsEnergyPriceInDiamonds(int seconds)
    {
        int amount;
        if (seconds <= minEnergyPriceInDiamondsBySeconds.Key)
        {
            amount = minEnergyPriceInDiamondsBySeconds.Value;
            return new ResourceData(ResourceId.Diamond, amount);
        }

        if (seconds >= maxEnergyPriceInDiamondsBySeconds.Key)
        {
            var mult = (float)seconds / maxEnergyPriceInDiamondsBySeconds.Key;
            amount = (int)(maxEnergyPriceInDiamondsBySeconds.Value * mult);
            return new ResourceData(ResourceId.Diamond, amount);
        }

        var lowerKVP = new KeyValuePair<int, int>();
        var upperKVP = new KeyValuePair<int, int>();
        foreach (var kvp in energyPriceInDiamondsBySeconds)
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
}
