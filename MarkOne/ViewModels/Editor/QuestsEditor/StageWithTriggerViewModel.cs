using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using MarkOne.Models.RegularDialogs;
using MarkOne.Models.UserControls;
using MarkOne.Scripts.GameCore.Quests.NextStageTriggers;
using MarkOne.Scripts.GameCore.Quests.QuestStages;
using MarkOne.Scripts.GameCore.Quests.StageActions;
using MarkOne.Views.UserControls;

namespace MarkOne.ViewModels.Editor.QuestsEditor;

public class StageWithTriggerViewModel : ViewModelBase
{
    private ObjectPropertiesEditorView? _selectedActionView;
    private ObjectPropertiesEditorView? _selectedTooltipView;
    private ObjectPropertiesEditorView? _selectedTriggerView;

    public QuestStageWithTrigger stage { get; }

    public ObservableCollection<ObjectPropertiesEditorView> actionViews { get; }
    public ObservableCollection<ObjectPropertiesEditorView> tooltipViews { get; }
    public ObservableCollection<ObjectPropertiesEditorView> triggerViews { get; }
    public ObjectPropertiesEditorView? selectedActionView
    {
        get => _selectedActionView;
        set => this.RaiseAndSetIfChanged(ref _selectedActionView, value);
    }
    public ObjectPropertiesEditorView? selectedTooltipView
    {
        get => _selectedTooltipView;
        set => this.RaiseAndSetIfChanged(ref _selectedTooltipView, value);
    }
    public ObjectPropertiesEditorView? selectedTriggerView
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

        actionViews = new ObservableCollection<ObjectPropertiesEditorView>();
        RefillActionsCollection();
        addNewActionCommand = ReactiveCommand.Create(AddNewAction);
        removeActionCommand = ReactiveCommand.Create(RemoveSelectedAction);

        tooltipViews = new ObservableCollection<ObjectPropertiesEditorView>();
        RefillTooltipsCollection();
        addNewTooltipCommand = ReactiveCommand.Create(AddNewTooltip);
        removeTooltipCommand = ReactiveCommand.Create(RemoveSelectedTooltip);

        triggerViews = new ObservableCollection<ObjectPropertiesEditorView>();
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
        SaveChanges();
        RegularDialogHelper.ShowItemSelectionDialog("Select action type:", new Dictionary<string, Action>()
        {
            {"Show Select Language Dialog", () => { stage.questActions.Add(new ShowLanguageSelectionDialogAction()); RefillActionsCollection(); } },
            {"Show Enter Name Dialog", () => { stage.questActions.Add(new ShowEnterNameDialogAction()); RefillActionsCollection(); } },
            {"Entry Town", () => { stage.questActions.Add(new EntryTownAction()); RefillActionsCollection(); } },
            {"Restore Full Health", () => { stage.questActions.Add(new RestoreFullHealthAction()); RefillActionsCollection(); } },
            {"Add Item To Inventory", () => { stage.questActions.Add(new AddItemToInventoryAction()); RefillActionsCollection(); } },
            {"Complete Quest", () => { stage.questActions.Add(new CompleteQuestAction()); RefillActionsCollection(); } },
            {"Start New Quest", () => { stage.questActions.Add(new StartNewQuestAction()); RefillActionsCollection(); } },
        });
    }

    public void RemoveSelectedAction()
    {
        if (_selectedActionView == null)
            return;

        var actionToRemove = _selectedActionView.vm.GetEditableObject<StageActionBase>();
        stage.questActions.Remove(actionToRemove);
        SaveChanges();
        RefillActionsCollection();
    }

    public void AddNewTooltip()
    {
        SaveChanges();
        stage.tooltips.Add(new Tooltip());
        RefillTooltipsCollection();
    }

    public void RemoveSelectedTooltip()
    {
        if (_selectedTooltipView == null)
            return;

        var tooltipToRemove = _selectedTooltipView.vm.GetEditableObject<Tooltip>();
        stage.tooltips.Remove(tooltipToRemove);
        SaveChanges();
        RefillTooltipsCollection();
    }

    public void AddNewTrigger()
    {
        SaveChanges();
        RegularDialogHelper.ShowItemSelectionDialog("Select trigger type:", new Dictionary<string, Action>()
        {
            {"Invoke From Code", () => { stage.nextStageTriggers.Add(new InvokeFromCodeTrigger()); RefillTriggersCollection(); } },
            {"Start Next Stage Immediate", () => { stage.nextStageTriggers.Add(new StartNextStageImmediateTrigger()); RefillTriggersCollection(); } },
            {"Continue Story Mode", () => { stage.nextStageTriggers.Add(new ContinueStoryModeTrigger()); RefillTriggersCollection(); } },
        });
    }

    public void RemoveSelectedTrigger()
    {
        if (_selectedTriggerView == null)
            return;

        var triggerToRemove = _selectedTriggerView.vm.GetEditableObject<TriggerBase>();
        stage.nextStageTriggers.Remove(triggerToRemove);
        SaveChanges();
        RefillTriggersCollection();
    }

    public void SaveChanges()
    {
        //foreach (var actionView in actionViews)
        //{
        //    actionView.vm.SaveObjectChanges();
        //}
        //foreach (var tooltipView in tooltipViews)
        //{
        //    tooltipView.vm.SaveObjectChanges();
        //}
        //foreach (var triggerView in triggerViews)
        //{
        //    triggerView.vm.SaveObjectChanges();
        //}
    }



}
