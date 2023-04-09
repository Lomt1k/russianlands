using System.Collections.Generic;
using System.Collections.ObjectModel;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using ReactiveUI;
using TextGameRPG.Views.UserControls;
using System.Reactive;
using TextGameRPG.Models.UserControls;
using TextGameRPG.Models.RegularDialogs;
using System;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Managers.GameDataBase;
using TextGameRPG.Scripts.GameCore.Managers;

namespace TextGameRPG.ViewModels.Editor.QuestsEditor
{
    public class StageWithBattleViewModel : ViewModelBase
    {
        private static readonly GameDataBase gameDataBase = Singletones.Get<GameDataBase>();


        private Dictionary<string, int> _mobIds = new Dictionary<string, int>();
        private string? _selectedMob;
        private ObjectFieldsEditorView? _selectedRewardView;

        public QuestStageWithBattle stage { get; }
        public ObservableCollection<string> mobsList { get; }
        public ObservableCollection<ObjectFieldsEditorView> rewardViews { get; }

        public string? selectedMob
        {
            get => _selectedMob;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedMob, value);
                if (value != null && _mobIds.ContainsKey(value))
                {
                    stage.mobId = _mobIds[value];
                }
            }
        }
        public ObjectFieldsEditorView? selectedRewardView
        {
            get => _selectedRewardView;
            set => this.RaiseAndSetIfChanged(ref _selectedRewardView, value);
        }

        public ReactiveCommand<Unit, Unit> addNewRewardCommand { get; }
        public ReactiveCommand<Unit, Unit> removeRewardCommand { get; }

        public StageWithBattleViewModel(QuestStageWithBattle stage)
        {
            this.stage = stage;

            mobsList = new ObservableCollection<string>();
            foreach (var mob in gameDataBase.mobs.GetAllData())
            {
                var mobStr = $"{mob.id} | {mob.debugName}";
                _mobIds[mobStr] = mob.id;
                mobsList.Add(mobStr);

                if (mob.id == stage.mobId)
                {
                    selectedMob = mobStr;
                }
            }

            rewardViews = new ObservableCollection<ObjectFieldsEditorView>();
            RefillRewardsCollection();
            addNewRewardCommand = ReactiveCommand.Create(AddNewReward);
            removeRewardCommand = ReactiveCommand.Create(RemoveSelectedReward);
        }

        public void RefillRewardsCollection()
        {
            UserControlsHelper.RefillObjectEditorsCollection(rewardViews, stage.rewards);
        }

        public void AddNewReward()
        {
            SaveChanges();
            RegularDialogHelper.ShowItemSelectionDialog("Select reward type:", new Dictionary<string, Action>()
            {
                {"Resource", () => { stage.rewards.Add(new ResourceReward()); RefillRewardsCollection(); } },
                {"Resource Range", () => { stage.rewards.Add(new ResourceRangeReward()); RefillRewardsCollection(); } },
                {"Resource AB Range", () => { stage.rewards.Add(new ResourceABWithOneBonusReward()); RefillRewardsCollection(); } },
                {"Item With Code", () => { stage.rewards.Add(new ItemWithCodeReward()); RefillRewardsCollection(); } },
                {"Random Item", () => { stage.rewards.Add(new RandomItemReward()); RefillRewardsCollection(); } },
            });
        }

        public void RemoveSelectedReward()
        {
            if (_selectedRewardView == null)
                return;

            var rewardToRemove = _selectedRewardView.vm.GetEditableObject<RewardBase>();
            stage.rewards.Remove(rewardToRemove);
            SaveChanges();
            RefillRewardsCollection();
        }

        public void SaveChanges()
        {
            foreach (var rewardView in rewardViews)
            {
                rewardView.vm.SaveObjectChanges();
            }
        }

    }
}
