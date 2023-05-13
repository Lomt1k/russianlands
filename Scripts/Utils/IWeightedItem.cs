using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkOne.Scripts.Utils;
public interface IWeightedItem
{
    int weight { get; }
}

public static class WeightedItemExtensions
{
    public static IWeightedItem GetRandom(this IEnumerable<IWeightedItem> items)
    {
        var sumWeights = items.Sum(x => x.weight);
        var resultWeight = new Random().Next(sumWeights);
        var iterationWeight = 0;
        foreach (var item in items)
        {
            iterationWeight += item.weight;
            if (resultWeight < iterationWeight)
            {
                return item;
            }
        }
        return items.Last();
    }
}
