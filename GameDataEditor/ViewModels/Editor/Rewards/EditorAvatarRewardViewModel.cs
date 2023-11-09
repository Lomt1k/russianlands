using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Units;
using System.Collections.ObjectModel;

namespace GameDataEditor.ViewModels.Editor.Rewards;
internal class EditorAvatarRewardViewModel : ViewModelBase
{
    public AvatarReward reward { get; }
    public ObservableCollection<AvatarId> avatarIds { get; } = new(System.Enum.GetValues<AvatarId>());

    public EditorAvatarRewardViewModel(AvatarReward _reward)
    {
        reward = _reward;
    }
}
