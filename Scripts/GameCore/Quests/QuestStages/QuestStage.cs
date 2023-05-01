﻿using Newtonsoft.Json;
using JsonKnownTypes;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    [JsonConverter(typeof(JsonKnownTypesConverter<QuestStage>))]
    public abstract class QuestStage
    {
        public int id { get; set; }
        public string comment { get; set; } = "New Stage";
        public int? jumpToStageIfNewSession { get; set; }

        [JsonIgnore]
        public QuestData quest { get; private set; }

        public override string ToString()
        {
            return $"{id} | {comment}";
        }

        public void SetupQuest(QuestData _quest)
        {
            quest = _quest;
        }

        public abstract Task InvokeStage(GameSession session);

    }


}
