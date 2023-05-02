﻿using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using MarkOne.Models;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Rewards;

namespace MarkOne.ViewModels.Rewards;

internal class EditorResourceRangeRewardViewModel : ViewModelBase
{
    private EnumValueModel<ResourceId> _selectedResourceId;

    public ResourceRangeReward reward { get; }

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


    public EditorResourceRangeRewardViewModel(ResourceRangeReward _reward)
    {
        reward = _reward;
        resourceIds = EnumValueModel<ResourceId>.CreateCollection();
        selectedResourceId = resourceIds.First(x => x.value == reward.resourceId);
    }
}
