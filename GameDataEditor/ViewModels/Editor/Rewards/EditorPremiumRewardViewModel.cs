using MarkOne.Scripts.GameCore.Rewards;

namespace GameDataEditor.ViewModels.Editor.Rewards;
internal class EditorPremiumRewardViewModel : ViewModelBase
{
    public PremiumReward reward { get; }

    public EditorPremiumRewardViewModel(PremiumReward _premiumReward)
    {
        reward = _premiumReward;
    }
}
