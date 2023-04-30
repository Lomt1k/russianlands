using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Rewards;

namespace TextGameRPG.ViewModels.Rewards
{
    internal class EditorResourceABWithOneBonusRewardViewModel : ViewModelBase
    {
        private EnumValueModel<ResourceType> _selectedResourceTypeA;
        private EnumValueModel<ResourceType> _selectedResourceTypeB;

        public ResourceABWithOneBonusReward reward { get; }
        public ObservableCollection<EnumValueModel<ResourceType>> resourceTypes { get; }
        public EnumValueModel<ResourceType> selectedResourceTypeA
        {
            get => _selectedResourceTypeA;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedResourceTypeA, value);
                reward.resourceTypeA = value.value;
            }
        }
        public EnumValueModel<ResourceType> selectedResourceTypeB
        {
            get => _selectedResourceTypeB;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedResourceTypeB, value);
                reward.resourceTypeB = value.value;
            }
        }


        public EditorResourceABWithOneBonusRewardViewModel(ResourceABWithOneBonusReward _reward)
        {
            reward = _reward;
            resourceTypes = EnumValueModel<ResourceType>.CreateCollection();
            selectedResourceTypeA = resourceTypes.First(x => x.value == reward.resourceTypeA);
            selectedResourceTypeB = resourceTypes.First(x => x.value == reward.resourceTypeB);
        }
    }
}
