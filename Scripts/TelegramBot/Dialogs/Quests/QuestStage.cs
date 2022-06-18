using System;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Characters;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Quests.ActionsOnStartStage;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests
{
    [Serializable]
    internal class QuestStage
    {
        public int id;
        public string comment = string.Empty;
        public int? jumpToStageIfNewSession = null;
        public int? jumpToStage;
        public Replica? replica = null;
        public Tooltip[]? tooltips = null;
        public ActionType[]? actions = null;
        public EndStageTrigger? endStageTrigger = null;
    }

    [Serializable]
    internal class Replica
    {
        public CharacterType characterType;
        public string localizationKey = string.Empty;
        public Answer[] answers = new Answer[0];
    }

    [Serializable]
    internal class Answer
    {
        public string comment = string.Empty;
        public string localizationKey = string.Empty;
        public int jumpToStage;
    }

    [Serializable]
    internal class Tooltip
    {
        public string comment = string.Empty;
        public string dialogType = string.Empty;
        public string localizationKey = string.Empty;
    }


}
