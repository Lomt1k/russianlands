using MarkOne.Scripts.GameCore.Rewards;

namespace GameDataEditor.ViewModels.Editor.Rewards;
internal class EditorRandomFruitsRewardViewModel : ViewModelBase
{
    public RandomFruitsReward reward { get; }

    public EditorRandomFruitsRewardViewModel(RandomFruitsReward _randomFruitsReward)
    {
        reward = _randomFruitsReward;
    }
}
