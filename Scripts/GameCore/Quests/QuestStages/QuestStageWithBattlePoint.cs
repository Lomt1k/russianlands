using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.Bot.Dialogs.Battle;
using TextGameRPG.Scripts.Bot.Dialogs.Town;
using TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Services.GameData;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    public class QuestStageWithBattlePoint : QuestStage
    {
        private static readonly GameDataHolder gameDataBase = Services.Services.Get<GameDataHolder>();

        public int mobId { get; set; }
        public int foodPrice { get; set; }
        public bool backButtonAvailable { get; set; } = true;
        public int nextStageIfWin { get; set; }
        public int nextStageIfLose { get; set; }
        public List<RewardBase> rewards { get; set; } = new List<RewardBase>();

        public override async Task InvokeStage(GameSession session)
        {
            if (session.currentDialog is TownDialog)
                return; // При старте новой сессии

            var battlePointData = GetMobBattlePointData(session);
            await new BattlePointDialog(session, battlePointData).Start();
        }

        public async Task InvokeStageWithStartBattleImmediate(GameSession session)
        {
            var battlePointData = GetMobBattlePointData(session);
            await new BattlePointDialog(session, battlePointData).SilentStart();
        }

        public BattlePointData GetMobBattlePointData(GameSession session)
        {
            var mobData = gameDataBase.mobs[mobId];

            // stage у квеста меняем сразу по окончанию боя, но вызываем его только после нажатия кнопки continue в окне наград
            var battlePointData = new BattlePointData
            {
                mob = new Mob(session, mobData),
                foodPrice = foodPrice,
                rewards = rewards,
                onBackButtonFunc = () => BackToMap(session),
                onBattleEndFunc = (Player player, BattleResult result) =>
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
                onContinueButtonFunc = async (Player player, BattleResult result) =>
                {
                    var questProgress = player.session.profile.dynamicData.quests;
                    var focusedQuestId = questProgress.GetFocusedQuest();
                    if (focusedQuestId != null)
                    {
                        var focusedQuest = gameDataBase.quests[focusedQuestId.Value];
                        var currentStage = focusedQuest.GetCurrentStage(player.session);
                        await currentStage.InvokeStage(player.session);
                    }
                }
            };

            if (!backButtonAvailable)
            {
                battlePointData.onBackButtonFunc = null;
            }

            return battlePointData;
        }

        private async Task BackToMap(GameSession session)
        {
            var questId = session.profile.dynamicData.quests.GetFocusedQuest();
            if (questId.HasValue)
            {
                var locationId = questId.Value.GetLocation();
                if (locationId.HasValue)
                {
                    await new MapDialog(session).StartWithLocation(locationId.Value).FastAwait();
                    return;
                }
            }

            await new MapDialog(session).Start();
        }

    }
}
