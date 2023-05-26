using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using MarkOne.Models;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Rewards;

namespace MarkOne.ViewModels.Editor.Rewards;

internal class EditorRandomItemRewardViewModel : ViewModelBase
{
    private EnumValueModel<Rarity> _selectedRarity;

    public RandomItemReward reward { get; }

    public ObservableCollection<EnumValueModel<Rarity>> rarities { get; }
    public EnumValueModel<Rarity> selectedRarity
    {
        get => _selectedRarity;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedRarity, value);
            reward.rarity = value.value;
        }
    }


    public EditorRandomItemRewardViewModel(RandomItemReward _reward)
    {
        reward = _reward;
        rarities = EnumValueModel<Rarity>.CreateCollection();
        selectedRarity = rarities.First(x => x.value == reward.rarity);
    }
}
