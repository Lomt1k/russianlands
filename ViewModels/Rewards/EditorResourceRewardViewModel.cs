using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Rewards;

namespace TextGameRPG.ViewModels.Rewards
{
    internal class EditorResourceRewardViewModel : ViewModelBase
    {
        private EnumValueModel<ResourceType> _selectedResourceType;

        public ResourceReward resourceReward { get; }

        public ObservableCollection<EnumValueModel<ResourceType>> resourceTypes { get; }
        public EnumValueModel<ResourceType> selectedResourceType 
        {
            get => _selectedResourceType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedResourceType, value);
                resourceReward.resourceType = value.value;
            }
        }
        

        public EditorResourceRewardViewModel(ResourceReward _resourceReward)
        {
            resourceReward = _resourceReward;
            resourceTypes = EnumValueModel<ResourceType>.CreateCollection();
            selectedResourceType = resourceTypes.First(x => x.value == resourceReward.resourceType);
        }
    }
}
