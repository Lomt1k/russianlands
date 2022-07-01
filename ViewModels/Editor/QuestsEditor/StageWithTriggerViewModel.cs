using System;
using System.Reactive;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ReactiveUI;
using TextGameRPG.Models.UserControls;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Views.Editor.UserControls;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Models.RegularDialogs;
using TextGameRPG.Scripts.GameCore.Quests.StageActions;

namespace TextGameRPG.ViewModels.Editor.QuestsEditor
{
    public class StageWithTriggerViewModel : ViewModelBase
    {
        private ObjectFieldsEditorView? _selectedActionView;
        private ObjectFieldsEditorView? _selectedTooltipView;
        private ObjectFieldsEditorView? _selectedTriggerView;

        public QuestStageWithTrigger stage { get; }

        public ObservableCollection<ObjectFieldsEditorView> actionViews { get; }
        public ObservableCollection<ObjectFieldsEditorView> tooltipViews { get; }
        public ObservableCollection<ObjectFieldsEditorView> triggerViews { get; }
        public ObjectFieldsEditorView? selectedActionView
        {
            get => _selectedActionView;
            set => this.RaiseAndSetIfChanged(ref _selectedActionView, value);
        }
        public ObjectFieldsEditorView? selectedTooltipView
        {
            get => _selectedTooltipView;
            set => this.RaiseAndSetIfChanged(ref _selectedTooltipView, value);
        }
        public ObjectFieldsEditorView? selectedTriggerView
        {
            get => _selectedTriggerView;
            set => this.RaiseAndSetIfChanged(ref _selectedTriggerView, value);
        }

        public ReactiveCommand<Unit, Unit> addNewActionCommand { get; }
        public ReactiveCommand<Unit, Unit> removeActionCommand { get; }
        public ReactiveCommand<Unit, Unit> addNewTooltipCommand { get; }
        public ReactiveCommand<Unit, Unit> removeTooltipCommand { get; }
        public ReactiveCommand<Unit, Unit> addNewTriggerCommand { get; }
        public ReactiveCommand<Unit, Unit> removeTriggerCommand { get; }

        public StageWithTriggerViewModel(QuestStageWithTrigger stage)
        {
            this.stage = stage;

            actionViews = new ObservableCollection<ObjectFieldsEditorView>();
            RefillActionsCollection();
            addNewActionCommand = ReactiveCommand.Create(AddNewAction);
            removeActionCommand = ReactiveCommand.Create(RemoveSelectedAction);

            tooltipViews = new ObservableCollection<ObjectFieldsEditorView>();
            RefillTooltipsCollection();
            addNewTooltipCommand = ReactiveCommand.Create(AddNewTooltip);
            removeTooltipCommand = ReactiveCommand.Create(RemoveSelectedTooltip);

            triggerViews = new ObservableCollection<ObjectFieldsEditorView>();
            RefillTriggersCollection();
            addNewTriggerCommand = ReactiveCommand.Create(AddNewTrigger);
            removeTriggerCommand = ReactiveCommand.Create(RemoveSelectedTrigger);
        }

        public void RefillActionsCollection()
        {
            UserControlsHelper.RefillObjectEditorsCollection(actionViews, stage.questActions);
        }

        public void RefillTooltipsCollection()
        {
            UserControlsHelper.RefillObjectEditorsCollection(tooltipViews, stage.tooltips);
        }

        public void RefillTriggersCollection()
        {
            UserControlsHelper.RefillObjectEditorsCollection(triggerViews, stage.nextStageTriggers);
        }

        public void AddNewAction()
        {
            RegularDialogHelper.ShowItemSelectionDialog("Select action type:", new Dictionary<string, Action>()
            {
                {"Show Select Language Dialog", () => { stage.questActions.Add(new ShowLanguageSelectionDialogAction()); RefillActionsCollection(); } },
                {"Show Enter Name Dialog", () => { stage.questActions.Add(new ShowEnterNameDialogAction()); RefillActionsCollection(); } },
                {"Entry Town", () => { stage.questActions.Add(new EntryTownAction()); RefillActionsCollection(); } },
                {"Restore Full Health", () => { stage.questActions.Add(new RestoreFullHealthAction()); RefillActionsCollection(); } },
            });
        }

        public void RemoveSelectedAction()
        {
            if (_selectedActionView == null)
                return;

            var actionToRemove = _selectedActionView.vm.GetEditableObject<StageActionBase>();
            stage.questActions.Remove(actionToRemove);
            RefillActionsCollection();
        }

        public void AddNewTooltip()
        {
            stage.tooltips.Add(new Tooltip());
            RefillTooltipsCollection();
        }

        public void RemoveSelectedTooltip()
        {
            if (_selectedTooltipView == null)
                return;

            var tooltipToRemove = _selectedTooltipView.vm.GetEditableObject<Tooltip>();
            stage.tooltips.Remove(tooltipToRemove);
            RefillTooltipsCollection();
        }

        public void AddNewTrigger()
        {
            RegularDialogHelper.ShowItemSelectionDialog("Select trigger type:", new Dictionary<string, Action>()
            {
                {"Invoke From Code", () => { stage.nextStageTriggers.Add(new InvokeFromCodeTrigger()); RefillTriggersCollection(); } },
            });
        }

        public void RemoveSelectedTrigger()
        {
            if (_selectedTriggerView == null)
                return;

            var triggerToRemove = _selectedTriggerView.vm.GetEditableObject<TriggerBase>();
            stage.nextStageTriggers.Remove(triggerToRemove);
            RefillTriggersCollection();
        }

        public void SaveChanges()
        {
            foreach (var actionView in actionViews)
            {
                actionView.vm.SaveObjectChanges();
            }
            foreach (var tooltipView in tooltipViews)
            {
                tooltipView.vm.SaveObjectChanges();
            }
            foreach (var triggerView in triggerViews)
            {
                triggerView.vm.SaveObjectChanges();
            }
        }



    }
}
