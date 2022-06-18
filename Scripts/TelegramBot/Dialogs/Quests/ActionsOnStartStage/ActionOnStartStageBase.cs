using System;
using Newtonsoft.Json;
using JsonKnownTypes;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests.ActionsOnStartStage
{
    [Serializable]
    [JsonConverter(typeof(JsonKnownTypesConverter<ActionOnStartStageBase>))]
    internal abstract class ActionOnStartStageBase
    {
        public abstract Task Execute(GameSession session);
    }
}
