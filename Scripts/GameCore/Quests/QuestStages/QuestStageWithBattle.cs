using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.Bot.Dialogs.Battle;
using TextGameRPG.Scripts.GameCore.Managers;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Managers.Battles;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    public class QuestStageWithBattle : QuestStage
    {
        private static readonly BattleManager battleManager = Singletones.Get<BattleManager>();

        public int mobId { get; set; }
        public int nextStageIfWin { get; set; }
        public int nextStageIfLose { get; set; }
        public List<RewardBase> rewards { get; set; } = new List<RewardBase>();

        public override Task InvokeStage(GameSession session)
        {
            var mobDB = GameDataBase.GameDataBase.instance.mobs;
            var mobData = mobDB[mobId];

            // stage у квеста меняем сразу по окончанию боя, но вызываем его только после нажатия кнопки continue в окне наград

            battleManager.StartBattleWithMob(session.player, mobData,
                rewards: rewards.Count > 0 ? rewards : null,
                onBattleEndFunc: (Player player, BattleResult result) =>
                {
                    var nextStage = result == BattleResult.Win ? nextStageIfWin : nextStageIfLose;
                    var questProgress = player.session.profile.dynamicData.quests;
                    var focusedQuestType = questProgress.GetFocusedQuest();
                    if (focusedQuestType != null)
                    {
                        questProgress.SetStage(focusedQuestType.Value, nextStage);
                    }
                    return Task.CompletedTask;
                },
                onContinueButtonFunc: async(Player player, BattleResult battleResult) =>
                {
                    var questProgress = player.session.profile.dynamicData.quests;
                    var focusedQuestType = questProgress.GetFocusedQuest();
                    if (focusedQuestType != null)
                    {
                        var focusedQuest = QuestsHolder.GetQuest(focusedQuestType.Value);
                        var currentStage = focusedQuest.GetCurrentStage(player.session);
                        await currentStage.InvokeStage(player.session);
                    }
                },
                isAvailableReturnToTownFunc: (Player player, BattleResult battleResult) => false );

            return Task.CompletedTask;
        }
    }
}
