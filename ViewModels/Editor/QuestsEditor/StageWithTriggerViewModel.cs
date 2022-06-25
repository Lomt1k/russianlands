using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TextGameRPG.Models.UserControls;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Views.Editor.UserControls;
using ReactiveUI;
using System.Reactive;
using Avalonia.Controls;

namespace TextGameRPG.ViewModels.Editor.QuestsEditor
{
    public class StageWithTriggerViewModel : ViewModelBase
    {
        private ObjectFieldsEditorView? _selectedTooltipView;

        public QuestStageWithTrigger stage { get; }

        public ObservableCollection<ObjectFieldsEditorView> tooltipViews { get; }
        public ObjectFieldsEditorView? selectedTooltipView
        {
            get => _selectedTooltipView;
            set => this.RaiseAndSetIfChanged(ref _selectedTooltipView, value);
        }

        public ReactiveCommand<Unit, Unit> addNewTooltipCommand { get; }
        public ReactiveCommand<Unit, Unit> removeTooltipCommand { get; }

        public StageWithTriggerViewModel(QuestStageWithTrigger stage)
        {
            this.stage = stage;

            tooltipViews = new ObservableCollection<ObjectFieldsEditorView>();
            RefillTooltipsCollection();
            addNewTooltipCommand = ReactiveCommand.Create(AddNewTooltip);
            removeTooltipCommand = ReactiveCommand.Create(RemoveSelectedTooltip);
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

        public void SaveChanges()
        {
            foreach (var tooltipView in tooltipViews)
            {
                tooltipView.viewModel.SaveObjectChanges();
            }
        }



    }
}
