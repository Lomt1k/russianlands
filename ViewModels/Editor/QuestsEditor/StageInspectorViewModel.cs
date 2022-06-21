using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using ReactiveUI;
using System.Reactive;

namespace TextGameRPG.ViewModels.Editor.QuestsEditor
{
    internal class StageInspectorViewModel : ViewModelBase
    {
        private QuestsEditorViewModel _questEditorVM;
        private QuestStage _stage;
        private bool _hasJumpToStageIfNewSession;
        private int? _jumpToStageIfNewSessionCache;

        public QuestStage stage
        {
            get => _stage;
            set => this.RaiseAndSetIfChanged(ref _stage, value);
        }

        public bool hasJumpToStageIfNewSession
        {
            get => _hasJumpToStageIfNewSession;
            set
            {
                this.RaiseAndSetIfChanged(ref _hasJumpToStageIfNewSession, value);
                if (value)
                {
                    _stage.jumpToStageIfNewSession = _jumpToStageIfNewSessionCache;
                }
                else
                {
                    _jumpToStageIfNewSessionCache = _stage.jumpToStageIfNewSession;
                    _stage.jumpToStageIfNewSession = null;
                }
            }
        }

        public ReactiveCommand<Unit, Unit> jumpToStageCheckedCommand { get; }
        public ReactiveCommand<Unit, Unit> jumpToStageUncheckedCommand { get; }

        public StageInspectorViewModel(QuestsEditorViewModel questEditorVM)
        {
            _questEditorVM = questEditorVM;
        }

        public void ShowStage(QuestStage stage)
        {
            this.stage = stage;
            _hasJumpToStageIfNewSession = stage.jumpToStageIfNewSession.HasValue;
            _jumpToStageIfNewSessionCache = stage.jumpToStageIfNewSession;
        }

    }
}
