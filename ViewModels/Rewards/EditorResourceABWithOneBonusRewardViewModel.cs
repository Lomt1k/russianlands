using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Rewards;

namespace TextGameRPG.ViewModels.Rewards;

internal class EditorResourceABWithOneBonusRewardViewModel : ViewModelBase
{
    private EnumValueModel<ResourceId> _selectedResourceIdA;
    private EnumValueModel<ResourceId> _selectedResourceIdB;

    public ResourceABWithOneBonusReward reward { get; }
    public ObservableCollection<EnumValueModel<ResourceId>> resourceIds { get; }
    public EnumValueModel<ResourceId> selectedResourceIdA
    {
        get => _selectedResourceIdA;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedResourceIdA, value);
            reward.resourceIdA = value.value;
        }
    }
    public EnumValueModel<ResourceId> selectedResourceIdB
    {
        get => _selectedResourceIdB;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedResourceIdB, value);
            reward.resourceIdB = value.value;
        }
    }


    public EditorResourceABWithOneBonusRewardViewModel(ResourceABWithOneBonusReward _reward)
    {
        reward = _reward;
        resourceIds = EnumValueModel<ResourceId>.CreateCollection();
        selectedResourceIdA = resourceIds.First(x => x.value == reward.resourceIdA);
        selectedResourceIdB = resourceIds.First(x => x.value == reward.resourceIdB);
    }
}
