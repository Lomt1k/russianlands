using System.Collections.ObjectModel;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Rewards;

namespace MarkOne.ViewModels.Editor.Rewards;

internal class EditorRandomItemRewardViewModel : ViewModelBase
{
    public RandomItemReward reward { get; }
    public ObservableCollection<Rarity> rarities { get; } = new(System.Enum.GetValues<Rarity>());
    public ObservableCollection<ItemType> itemTypes { get; } = new(System.Enum.GetValues<ItemType>());


    public EditorRandomItemRewardViewModel(RandomItemReward _reward)
    {
        reward = _reward;
    }
}
