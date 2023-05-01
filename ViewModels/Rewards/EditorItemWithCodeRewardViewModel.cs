using TextGameRPG.Scripts.GameCore.Rewards;

namespace TextGameRPG.ViewModels.Rewards
{
    internal class EditorItemWithCodeRewardViewModel : ViewModelBase
    {
        public ItemWithCodeReward reward { get; }

        public EditorItemWithCodeRewardViewModel(ItemWithCodeReward _reward)
        {
            reward = _reward;
        }
    }
}
