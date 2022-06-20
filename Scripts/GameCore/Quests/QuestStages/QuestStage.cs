using Newtonsoft.Json;
using JsonKnownTypes;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonConverter(typeof(JsonKnownTypesConverter<QuestStage>))]
    internal abstract class QuestStage
    {
        public int id { get; set; }
        public string comment { get; set; } = "New Stage";
        public int? jumpToStageIfNewSession { get; set; }

        public override string ToString()
        {
            return $"{id} | {comment}";
        }
    }


}
