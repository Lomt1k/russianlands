﻿using System;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Characters;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests.QuestStages
{
    [Serializable]
    internal class QuestStageWithReplica : QuestStage
    {
        public Replica? replica = null;
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
        public int nextStage;
    }


}
