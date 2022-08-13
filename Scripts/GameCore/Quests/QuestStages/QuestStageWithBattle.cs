using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.TelegramBot.Managers;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    public class QuestStageWithBattle : QuestStage
    {
        public int mobId { get; set; }
        public int nextStageIfWin { get; set; }
        public int nextStageIfLose { get; set; }
        public List<RewardBase> rewards { get; set; } = new List<RewardBase>();

        public override Task InvokeStage(GameSession session)
        {
            var mobDB = GameDataBase.GameDataBase.instance.mobs;
            var mobData = mobDB[mobId];
            GlobalManagers.battleManager?.StartBattle(session.player, mobData);

            return Task.CompletedTask;
        }
    }
}
