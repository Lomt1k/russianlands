using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using System.Text;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Rewards;

[JsonObject]
public class ResourceABWithOneBonusReward : ResourceRewardBase
{
    public ResourceId resourceIdA { get; set; } = ResourceId.Gold;
    public int amountA { get; set; }

    public ResourceId resourceIdB { get; set; } = ResourceId.Wood;
    public int amountB { get; set; }

    public int bonusA_min { get; set; }
    public int bonusA_max { get; set; }
    public int bonusB_min { get; set; }
    public int bonusB_max { get; set; }


    public override Task<string?> AddReward(GameSession session)
    {
        var rewards = PrepareRewards();
        session.player.resources.ForceAdd(rewards);
        var text = rewards.GetLocalizedView(session);
        return Task.FromResult(text);
    }

    public override Task<string?> AddRewardWithAddPossiblePremiumRewardToList(GameSession session, List<ResourceReward> possiblePremiumRewards)
    {
        var rewards = PrepareRewards();
        foreach (var (resourceId, amount) in rewards)
        {
            var premiumAmount = amount * 15 / 100;
            if (premiumAmount > 0)
            {
                possiblePremiumRewards.Add(new ResourceReward(resourceId, premiumAmount));
            }
        }
        session.player.resources.ForceAdd(rewards);
        var text = rewards.GetLocalizedView(session);
        return Task.FromResult(text);
    }

    private ResourceData[] PrepareRewards()
    {
        var isA = new Random().Next(2) == 0;
        return isA ? GetRewardsWithBonusA() : GetRewardsWithBonusB();
    }

    private ResourceData[] GetRewardsWithBonusA()
    {
        var amountWithBonus = amountA + new Random().Next(bonusA_min, bonusA_max + 1);
        var resourceDatas = new ResourceData[]
        {
            new ResourceData(resourceIdA, amountWithBonus),
            new ResourceData(resourceIdB, amountB),
        };
        return resourceDatas;
    }

    private ResourceData[] GetRewardsWithBonusB()
    {
        var amountWithBonus = amountB + new Random().Next(bonusB_min, bonusB_max + 1);
        var resourceDatas = new ResourceData[]
        {
            new ResourceData(resourceIdA, amountA),
            new ResourceData(resourceIdB, amountWithBonus),
        };
        return resourceDatas;
    }

    public override string GetPossibleRewardsView(GameSession session)
    {
        var sb = new StringBuilder();
        var minResourceA = new ResourceData(resourceIdA, amountA);
        var minResourceB = new ResourceData(resourceIdB, amountB);
        sb.AppendLine(minResourceA.GetLocalizedView(session) + '+');
        sb.Append(minResourceB.GetLocalizedView(session) + '+');
        return sb.ToString();
    }

}
