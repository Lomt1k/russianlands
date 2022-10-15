using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using System.Runtime.Serialization;
using System.Linq;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;

namespace TextGameRPG.Scripts.GameCore.Quests
{
    [Serializable]
    public class Quest
    {
        private const int STAGE_FIRST = 100;

        public QuestType questType;
        public List<QuestStage> stages = new List<QuestStage>();

        protected Dictionary<int, QuestStage> _stagesById = new Dictionary<int, QuestStage>();

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            _stagesById = stages.ToDictionary(x => x.id);
        }

        public int GetCurrentStageId(GameSession session)
        {
            return session.profile.dynamicData.quests.GetStage(questType);
        }

        public QuestStage GetCurrentStage(GameSession session)
        {
            var stageId = GetCurrentStageId(session);
            return _stagesById[stageId];
        }

        public async Task CompleteQuest(GameSession session)
        {
            if (IsStarted(session) && !IsCompleted(session))
            {
                await SetStage(session, -1);
            }
        }

        public async Task SetStage(GameSession session, int stageId)
        {
            if (stageId <= 0)
            {
                session.profile.dynamicData.quests.SetStage(questType, stageId, false);
                return;
            }

            var stage = _stagesById[stageId];
            bool isFocusRequired = IsFocusRequired(stage);
            session.profile.dynamicData.quests.SetStage(questType, stageId, isFocusRequired);
            await stage.InvokeStage(session);
        }

        public bool IsCompleted(GameSession session)
        {
            return session.profile.dynamicData.quests.IsCompleted(questType);
        }

        public bool IsStarted(GameSession session)
        {
            return session.profile.dynamicData.quests.IsStarted(questType);
        }

        public async Task StartQuest(GameSession session)
        {
            await SetStage(session, STAGE_FIRST);
        }

        public bool IsFocusRequired(GameSession session)
        {
            if (!IsStarted(session) || IsCompleted(session))
                return false;

            var stageId = GetCurrentStageId(session);
            var stage = _stagesById[stageId];
            return IsFocusRequired(stage);
        }

        public bool IsFocusRequired(QuestStage stage)
        {
            switch (stage)
            {
                case QuestStageWithReplica withReplica:
                    return true;
                case QuestStageWithBattle withBattle:
                    return true;
                case QuestStageWithBattlePoint withBattlePoint:
                    return false;
                case QuestStageWithTrigger withTrigger:
                    return withTrigger.isFocusRequired;
                default:
                    return false;
            }
        }

    }
}
