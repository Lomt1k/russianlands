using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using MarkOne.Models;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Rewards;

namespace GameDataEditor.ViewModels.Editor.Rewards;

internal class EditorResourceRewardViewModel : ViewModelBase
{
    private EnumValueModel<ResourceId> _selectedResourceId;

    public ResourceReward reward { get; }

    public ObservableCollection<EnumValueModel<ResourceId>> resourceIds { get; }
    public EnumValueModel<ResourceId> selectedResourceId
    {
        get => _selectedResourceId;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedResourceId, value);
            reward.resourceId = value.value;
        }
    }


    public EditorResourceRewardViewModel(ResourceReward _resourceReward)
    {
        reward = _resourceReward;
        resourceIds = EnumValueModel<ResourceId>.CreateCollection();
        selectedResourceId = resourceIds.First(x => x.value == reward.resourceId);
    }
}
