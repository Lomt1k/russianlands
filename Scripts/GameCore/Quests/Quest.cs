using System.Collections.Generic;
using TextGameRPG.Scripts.Bot.Sessions;
using System.Runtime.Serialization;
using System.Linq;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Quests
{
    [JsonObject]
    public class Quest
    {
        private const int STAGE_FIRST = 100;

        public QuestId questId;
        public List<QuestStage> stages = new List<QuestStage>();

        [JsonIgnore]
        protected Dictionary<int, QuestStage> _stagesById = new Dictionary<int, QuestStage>();

        [JsonIgnore]
        public int battlePointsCount { get; private set; }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            _stagesById = stages.ToDictionary(x => x.id);
            battlePointsCount = stages.Where(x => x is QuestStageWithBattlePoint).Count();

            foreach (var stage in stages)
            {
                stage.SetupQuest(this);
            }
        }

        public int GetCurrentStageId(GameSession session)
        {
            return session.profile.dynamicData.quests.GetStage(questId);
        }

        public QuestStage GetCurrentStage(GameSession session)
        {
            var stageId = GetCurrentStageId(session);
            return _stagesById[stageId];
        }

        public bool TryGetStageById(int stageId, out QuestStage? questStage)
        {
            return _stagesById.TryGetValue(stageId, out questStage);
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
            session.profile.dynamicData.quests.SetStage(questId, stageId);
            if (stageId <= 0)
                return;

            var stage = _stagesById[stageId];
            await stage.InvokeStage(session);
        }

        public bool IsCompleted(GameSession session)
        {
            return session.profile.dynamicData.quests.IsCompleted(questId);
        }

        public bool IsStarted(GameSession session)
        {
            return session.profile.dynamicData.quests.IsStarted(questId);
        }

        public async Task StartQuest(GameSession session)
        {
            await SetStage(session, STAGE_FIRST);
        }

        public int GetCompletedBattlePoints(GameSession session)
        {
            if (!IsStarted(session))
                return 0;
            if (IsCompleted(session))
                return battlePointsCount;

            var currentStage = GetCurrentStageId(session);
            int completedPoints = 0;
            foreach (var stage in _stagesById)
            {
                if (stage.Key >= currentStage)
                    break;

                if (stage.Value is QuestStageWithBattlePoint)
                {
                    completedPoints++;
                }
            }
            return completedPoints;
        }

    }
}
