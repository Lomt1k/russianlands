using MarkOne.Scripts.GameCore.Rewards;

namespace MarkOne.ViewModels.Editor.Rewards;

internal class EditorItemWithCodeRewardViewModel : ViewModelBase
{
    public ItemWithCodeReward reward { get; }

    public EditorItemWithCodeRewardViewModel(ItemWithCodeReward _reward)
    {
        reward = _reward;
    }
}
