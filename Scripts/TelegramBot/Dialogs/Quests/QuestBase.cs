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
    [JsonConverter(typeof(JsonKnownTypesConverter<QuestBase>))]
    internal abstract class QuestBase
    {
        private const int STAGE_FINISHED = -1;
        private const int STAGE_NOT_STARTED = 0;
        private const int STAGE_FIRST = 100;

        public List<QuestStage> stages = new List<QuestStage>();

        [JsonIgnore]
        public abstract QuestType questType { get; }

        protected Dictionary<int, QuestStage> _stagesById = new Dictionary<int, QuestStage>();

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            _stagesById = stages.ToDictionary(x => x.id);
        }

        public abstract int GetCurrentStage(GameSession session);

        protected abstract void SetCurrentStageInProfile(GameSession session, int stage);

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
