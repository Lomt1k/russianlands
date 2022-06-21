using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using ReactiveUI;

namespace TextGameRPG.ViewModels.Editor.QuestsEditor
{
    internal class StageInspectorViewModel : ViewModelBase
    {
        private QuestStage? _stage;
        private bool _hasJumpToStageIfNewSession;
        private int? _jumpToStageIfNewSessionCache;

        public QuestStage? stage
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
                if (value && _jumpToStageIfNewSessionCache.HasValue)
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

        public StageInspectorViewModel()
        {
        }

        public void ShowStage(QuestStage stage)
        {
            this.stage = stage;
            _jumpToStageIfNewSessionCache = stage.jumpToStageIfNewSession;
            hasJumpToStageIfNewSession = stage.jumpToStageIfNewSession.HasValue;
        }

    }
}
