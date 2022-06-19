using System;
using Newtonsoft.Json;
using JsonKnownTypes;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [Serializable]
    [JsonConverter(typeof(JsonKnownTypesConverter<QuestStage>))]
    internal abstract class QuestStage
    {
        public int id;
        public string comment = string.Empty;
        public int? jumpToStageIfNewSession = null;
    }


}
