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
    internal class Quest
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

        public int GetCurrentStage(GameSession session)
        {
            return session.profile.dynamicData.quests.GetStage(questType);
        }

        public async Task SetStage(GameSession session, int stageId)
        {
            session.profile.dynamicData.quests.SetStage(questType, stageId);
            var stage = _stagesById[stageId];
            //TODO
        }

        public bool IsCompleted(GameSession session)
        {
            return session.profile.dynamicData.quests.IsFinished(questType);
        }

        public bool IsStarted(GameSession session)
        {
            return session.profile.dynamicData.quests.IsStarted(questType);
        }

        public async Task StartQuest(GameSession session)
        {
            await SetStage(session, STAGE_FIRST);
        }

    }
}
