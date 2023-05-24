using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;
using System.Text;

public static class RewardExtensions
{
    public static string GetPossibleRewardsView(this IEnumerable<RewardBase> rewards, GameSession session)
    {
        var sb = new StringBuilder();
        foreach (var reward in rewards)
        {
            sb.AppendLine(reward.GetPossibleRewardsView(session));
        }
        return sb.ToString();
    }
}
