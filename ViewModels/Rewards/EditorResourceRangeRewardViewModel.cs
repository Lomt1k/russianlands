using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Rewards;

namespace TextGameRPG.ViewModels.Rewards
{
    internal class EditorResourceRangeRewardViewModel : ViewModelBase
    {
        private EnumValueModel<ResourceType> _selectedResourceType;

        public ResourceRangeReward reward { get; }

        public ObservableCollection<EnumValueModel<ResourceType>> resourceTypes { get; }
        public EnumValueModel<ResourceType> selectedResourceType
        {
            get => _selectedResourceType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedResourceType, value);
                reward.resourceType = value.value;
            }
        }


        public EditorResourceRangeRewardViewModel(ResourceRangeReward _reward)
        {
            reward = _reward;
            resourceTypes = EnumValueModel<ResourceType>.CreateCollection();
            selectedResourceType = resourceTypes.First(x => x.value == reward.resourceType);
        }
    }
}
