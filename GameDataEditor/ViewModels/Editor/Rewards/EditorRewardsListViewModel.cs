using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameDataEditor.Models.RegularDialogs;
using MarkOne.Scripts.GameCore.Rewards;
using GameDataEditor.ViewModels.UserControls;
using GameDataEditor.Views.Editor.Rewards;
using MarkOne.Scripts.GameCore.Resources;

namespace GameDataEditor.ViewModels.Editor.Rewards;

internal sealed class EditorRewardsListViewModel : EditorListViewModel<RewardBase>
{
    protected override async Task<RewardBase?> CreateNewListItem()
    {
        RewardBase? result = null;
        await RegularDialogHelper.ShowItemSelectionDialog("Select reward type:", new Dictionary<string, Action>()
        {
            {"Resource", () => result = new ResourceReward(ResourceId.Gold, 0) },
            {"Resource Range", () => result = new ResourceRangeReward(ResourceId.Gold, 0, 0) },
            {"Resource AB With One Bonus", () => result = new ResourceABWithOneBonusReward() },
            {"Item With Code", () => result = new ItemWithCodeReward() },
            {"Random Item", () => result = new RandomItemReward() },
            {"Random Fruits", () => result = new RandomFruitsReward() },
            {"Premium", () => result = new PremiumReward() },
            {"Avatar", () => result = new AvatarReward() },
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
            RandomFruitsReward reward => new EditorRandomFruitsRewardView { DataContext = new EditorRandomFruitsRewardViewModel(reward) },
            PremiumReward reward => new EditorPremiumRewardView() { DataContext = new EditorPremiumRewardViewModel(reward) },
            AvatarReward reward => new EditorAvatarRewardView() { DataContext = new EditorAvatarRewardViewModel(reward) },
        };
    }
}
