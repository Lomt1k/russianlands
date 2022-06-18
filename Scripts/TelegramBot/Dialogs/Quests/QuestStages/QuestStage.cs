using System;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Characters;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Quests.ActionsOnStartStage;
using Newtonsoft.Json;
using JsonKnownTypes;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests.QuestStages
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
