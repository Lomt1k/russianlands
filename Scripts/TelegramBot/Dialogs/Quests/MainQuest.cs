using System;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests
{
    [Serializable]
    internal class MainQuest : QuestBase
    {
        public override QuestType questType => QuestType.MainQuest;

        public override int GetCurrentStage(GameSession session)
        {
            return session.profile.data.mainQuestStage;
        }

        protected override void SetCurrentStageInProfile(GameSession session, int stage)
        {
            session.profile.data.mainQuestStage = stage;
        }
    }
}
