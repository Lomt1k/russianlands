using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Rewards;
public class RandomFruitsReward : RewardBase
{
    public int amount { get; set; }

    public override Task<string> AddReward(GameSession session)
    {
        var allFruitTypes = ResourcesDictionary.GetFruitTypes().ToArray();
        var fruits = new Dictionary<ResourceId,int>();
        var random = new Random();
        for (int i = 0; i < amount; i++)
        {
            var nextFruitIndex = random.Next(allFruitTypes.Length);
            var nextFruitType = allFruitTypes[nextFruitIndex];
            if (fruits.ContainsKey(nextFruitType))
            {
                fruits[nextFruitType]++;
            }
            else
            {
                fruits.Add(nextFruitType, 1);
            }

        }

        var resourceDatas = new ResourceData[fruits.Count];
        int j = 0;
        foreach (var (resourceId, amount) in fruits)
        {
            resourceDatas[j] = new ResourceData(resourceId, amount);
            j++;
        }

        session.player.resources.ForceAdd(resourceDatas);
        var result = resourceDatas.Length == 1
            ? resourceDatas[0].GetLocalizedView(session)
            : resourceDatas.GetLocalizedView(session);
        return Task.FromResult(result);
    }

    public override string GetPossibleRewardsView(GameSession session)
    {
        return amount == 1
            ? Emojis.ResourceFruitApple + Localization.Get(session, "reward_random_fruit")
            : Emojis.ResourceFruitApple + Localization.Get(session, "reward_random_fruits", amount);
    }
}
