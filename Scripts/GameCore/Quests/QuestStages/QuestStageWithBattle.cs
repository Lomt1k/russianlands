using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Dialogs.Battle;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Quests.QuestStages;

[JsonObject]
public class QuestStageWithBattle : QuestStage
{
    private static readonly BattleManager battleManager = Services.Services.Get<BattleManager>();
    private static readonly GameDataHolder gameDataBase = Services.Services.Get<GameDataHolder>();

    public int mobId { get; set; }
    public int nextStageIfWin { get; set; }
    public int nextStageIfLose { get; set; }
    public List<RewardBase> rewards { get; set; } = new List<RewardBase>();

    public override Task InvokeStage(GameSession session)
    {
        var mobData = gameDataBase.mobs[mobId];

        // stage у квеста меняем сразу по окончанию боя, но вызываем его только после нажатия кнопки continue в окне наград

        battleManager.StartBattleWithMob(session.player, mobData,
            rewards: rewards.Count > 0 ? rewards : null,
            onBattleEndFunc: (Player player, BattleResult result) =>
            {
                var nextStage = result == BattleResult.Win ? nextStageIfWin : nextStageIfLose;
                var questProgress = player.session.profile.dynamicData.quests;
                var focusedQuestId = questProgress.GetFocusedQuest();
                if (focusedQuestId != null)
                {
                    questProgress.SetStage(focusedQuestId.Value, nextStage);
                }
                return Task.CompletedTask;
            },
            onContinueButtonFunc: async (Player player, BattleResult battleResult) =>
            {
                var questProgress = player.session.profile.dynamicData.quests;
                var focusedQuestId = questProgress.GetFocusedQuest();
                if (focusedQuestId != null)
                {
                    var focusedQuest = gameDataBase.quests[focusedQuestId.Value];
                    var currentStage = focusedQuest.GetCurrentStage(player.session);
                    await currentStage.InvokeStage(player.session);
                }
            },
            isAvailableReturnToTownFunc: (Player player, BattleResult battleResult) => false);

        return Task.CompletedTask;
    }
}
