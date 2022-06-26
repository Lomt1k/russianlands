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
            actionViews.Clear();
            foreach (var action in stage.questActions)
            {
                var actionView = UserControlsHelper.CreateObjectFieldsEditor(action);
                actionViews.Add(actionView);
            }
        }

        public void RefillTooltipsCollection()
        {
            tooltipViews.Clear();
            foreach (var tooltip in stage.tooltips)
            {
                var tooltipView = UserControlsHelper.CreateObjectFieldsEditor(tooltip);
                tooltipViews.Add(tooltipView);
            }
        }

        public void RefillTriggersCollection()
        {
            triggerViews.Clear();
            foreach (var trigger in stage.nextStageTriggers)
            {
                var triggerView = UserControlsHelper.CreateObjectFieldsEditor(trigger);
                triggerViews.Add(triggerView);
            }
        }

        public void AddNewAction()
        {
            RegularDialogHelper.ShowItemSelectionDialog("Select action type:", new Dictionary<string, Action>()
            {
                {"Entry Town", () => { stage.questActions.Add(new EntryTownAction()); RefillActionsCollection(); } },
                {"Restore Full Health", () => { stage.questActions.Add(new RestoreFullHealthAction()); RefillActionsCollection(); } },
            });
        }

        public void RemoveSelectedAction()
        {
            if (_selectedActionView == null)
                return;

            var actionToRemove = (StageActionBase)_selectedActionView.viewModel.editableObject;
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

            var tooltipToRemove = (Tooltip)_selectedTooltipView.viewModel.editableObject;
            stage.tooltips.Remove(tooltipToRemove);
            RefillTooltipsCollection();
        }

        public void AddNewTrigger()
        {
            RegularDialogHelper.ShowItemSelectionDialog("Select trigger type:", new Dictionary<string, Action>()
            {
                {"On Dialog Close", () => { stage.nextStageTriggers.Add(new OnDialogCloseTrigger()); RefillTriggersCollection(); } },
            });
        }

        public void RemoveSelectedTrigger()
        {
            if (_selectedTriggerView == null)
                return;

            var triggerToRemove = (TriggerBase)_selectedTriggerView.viewModel.editableObject;
            stage.nextStageTriggers.Remove(triggerToRemove);
            RefillTriggersCollection();
        }

        public void SaveChanges()
        {
            foreach (var actionView in actionViews)
            {
                actionView.viewModel.SaveObjectChanges();
            }
            foreach (var tooltipView in tooltipViews)
            {
                tooltipView.viewModel.SaveObjectChanges();
            }
            foreach (var triggerView in triggerViews)
            {
                triggerView.viewModel.SaveObjectChanges();
            }
        }



    }
}
