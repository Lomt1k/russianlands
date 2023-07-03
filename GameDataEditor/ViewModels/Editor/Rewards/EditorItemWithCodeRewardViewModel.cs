using MarkOne.Scripts.GameCore.Rewards;

namespace GameDataEditor.ViewModels.Editor.Rewards;

internal class EditorItemWithCodeRewardViewModel : ViewModelBase
{
    public ItemWithCodeReward reward { get; }

    public EditorItemWithCodeRewardViewModel(ItemWithCodeReward _reward)
    {
        reward = _reward;
    }
}
