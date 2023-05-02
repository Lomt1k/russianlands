using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkOne.Models.RegularDialogs;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.ViewModels.UserControls;
using MarkOne.Views.Editor.Rewards;

namespace MarkOne.ViewModels.Rewards;

internal sealed class EditorRewardsListViewModel : EditorListViewModel<RewardBase>
{
    protected override async Task<RewardBase?> CreateNewListItem()
    {
        RewardBase? result = null;
        await RegularDialogHelper.ShowItemSelectionDialog("Select reward type:", new Dictionary<string, Action>()
        {
            {"Resource", () => result = new ResourceReward() },
            {"Resource Range", () => result = new ResourceRangeReward() },
            {"Resource AB With One Bonus", () => result = new ResourceABWithOneBonusReward() },
            {"Item With Code", () => result = new ItemWithCodeReward() },
            {"Random Item", () => result = new RandomItemReward() },
        });
        return result;
    }

    protected override UserControl CreateViewForItem(RewardBase item)
    {
        return item switch
        {
            ResourceReward reward => new EditorResourceRewardView() { DataContext = new EditorResourceRewardViewModel(reward) },
            ResourceRangeReward reward => new EditorResourceRangeRewardView() { DataContext = new EditorResourceRangeRewardViewModel(reward) },
            ResourceABWithOneBonusReward reward => new EditorResourceABWithOneBonusRewardView() { DataContext = new EditorResourceABWithOneBonusRewardViewModel(reward) },
            ItemWithCodeReward reward => new EditorItemWithCodeRewardView() { DataContext = new EditorItemWithCodeRewardViewModel(reward) },
            RandomItemReward reward => new EditorRandomItemRewardView() { DataContext = new EditorRandomItemRewardViewModel(reward) },
        };
    }
}
