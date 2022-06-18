using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using System.Runtime.Serialization;
using System.Linq;
using System.Threading.Tasks;
using JsonKnownTypes;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Quests.QuestStages;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests
{
    [Serializable]
    internal class Quest
    {
        private const int STAGE_FINISHED = -1;
        private const int STAGE_NOT_STARTED = 0;
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
            var questProgress = session.profile.questProgressData;
            var fieldName = questType.ToString();
            var field = questProgress.GetType().GetField(fieldName);

            if (field == null)
            {
                Program.logger.Error($"QuestProgress database table: Not found field with name '{fieldName}'");
                return STAGE_FINISHED;
            }

            var stage = (int)field.GetValue(questProgress);
            return stage;
        }

        protected void SetCurrentStageInProfile(GameSession session, int stage)
        {
            var questProgress = session.profile.questProgressData;
            var fieldName = questType.ToString();
            var field = questProgress.GetType().GetField(fieldName);

            if (field == null)
            {
                Program.logger.Error($"QuestProgress database table: Not found field with name '{fieldName}'");
                return;
            }            
            field.SetValue(questProgress, stage);
        }

        public async Task SetStage(GameSession session, int stageId)
        {
            SetCurrentStageInProfile(session, stageId);

            var stage = _stagesById[stageId];
            //TODO
        }

        public bool IsCompleted(GameSession session)
        {
            return GetCurrentStage(session) == STAGE_FINISHED;
        }

        public bool IsStarted(GameSession session)
        {
            return GetCurrentStage(session) != STAGE_NOT_STARTED;
        }

        public async Task StartQuest(GameSession session)
        {
            await SetStage(session, STAGE_FIRST);
        }

    }
}
