﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.Bot.Dialogs.Battle;
using TextGameRPG.Scripts.Bot.Dialogs.Town;
using TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    [JsonObject]
    public class QuestStageWithBattlePoint : QuestStage
    {
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
            await new MobBattlePointDialog(session, battlePointData).Start();
        }

        public async Task InvokeStageWithStartBattleImmediate(GameSession session)
        {
            var battlePointData = GetMobBattlePointData(session);
            await new MobBattlePointDialog(session, battlePointData).SilentStart();
        }

        private MobBattlePointData GetMobBattlePointData(GameSession session)
        {
            var mobDB = GameDataBase.GameDataBase.instance.mobs;
            var mobData = mobDB[mobId];

            // stage у квеста меняем сразу по окончанию боя, но вызываем его только после нажатия кнопки continue в окне наград
            var battlePointData = new MobBattlePointData
            {
                mob = new Mob(session, mobData),
                foodPrice = foodPrice,
                rewards = rewards,
                onBackButtonFunc = () => BackToMap(session),
                onBattleEndFunc = (Player player, BattleResult result) =>
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
                onContinueButtonFunc = async (Player player, BattleResult result) =>
                {
                    var questProgress = player.session.profile.dynamicData.quests;
                    var focusedQuestType = questProgress.GetFocusedQuest();
                    if (focusedQuestType != null)
                    {
                        var focusedQuest = QuestsHolder.GetQuest(focusedQuestType.Value);
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
            var questType = session.profile.dynamicData.quests.GetFocusedQuest();
            if (questType.HasValue)
            {
                var locationType = questType.Value.GetLocation();
                if (locationType.HasValue)
                {
                    await new LocationMapDialog(session, locationType.Value).Start();
                    return;
                }
            }

            await new GlobalMapDialog(session).Start();
        }

    }
}
