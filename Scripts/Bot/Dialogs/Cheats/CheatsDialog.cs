using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Commands;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Cheats
{
    public class CheatsDialog : DialogBase
    {
        public CheatsDialog(GameSession _session) : base(_session)
        {
        }

        public override async Task Start()
        {
            ClearButtons();
            RegisterButton("Resources", () => ShowResourcesGroup());
            RegisterButton("Buildings", () => ShowBuildingsGroup());
            RegisterButton("Quest Progress", () => ShowQuestProgressGroup());
            RegisterTownButton(isDoubleBack: false);

            var header = "Cheats".Bold();
            await SendDialogMessage(header, GetKeyboardWithRowSizes(2, 1, 1))
                .ConfigureAwait(false);
        }

        #region Resources Group

        public async Task ShowResourcesGroup()
        {
            ClearButtons();
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                RegisterButton(resourceType.ToString(), () => SelectAmountForAddResource(resourceType));
            }
            RegisterBackButton("Cheats", () => Start());
            RegisterTownButton(isDoubleBack: true);

            await SendDialogMessage("Resources".Bold(), GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        public async Task SelectAmountForAddResource(ResourceType resourceType)
        {
            ClearButtons();
            RegisterButton("10", () => InvokeAddResourceCommand(resourceType, 10));
            RegisterButton("50", () => InvokeAddResourceCommand(resourceType, 50));
            RegisterButton("100", () => InvokeAddResourceCommand(resourceType, 100));
            RegisterButton("1k", () => InvokeAddResourceCommand(resourceType, 1_000));
            RegisterButton("10k", () => InvokeAddResourceCommand(resourceType, 10_000));
            RegisterButton("100k", () => InvokeAddResourceCommand(resourceType, 100_000));
            RegisterButton("1kk", () => InvokeAddResourceCommand(resourceType, 1_000_000));
            RegisterButton("10kk", () => InvokeAddResourceCommand(resourceType, 10_000_000));
            RegisterButton("MAX", () => InvokeAddResourceCommand(resourceType, int.MaxValue));
            RegisterBackButton("Resources", () => ShowResourcesGroup());
            RegisterDoubleBackButton("Cheats", () => Start());

            var text = $"{resourceType} | Add amount:";
            await SendDialogMessage(text, GetKeyboardWithRowSizes(3, 3, 3, 2))
                .ConfigureAwait(false);
        }

        public async Task InvokeAddResourceCommand(ResourceType resourceType, int amount)
        {
            await CommandHandler.HandleCommand(session, $"/addresource {resourceType} {amount}")
                .ConfigureAwait(false);
            await Start()
                .ConfigureAwait(false);
        }

        #endregion

        #region Buildings Group

        private async Task ShowBuildingsGroup()
        {
            ClearButtons();

            RegisterButton("Townhall + Storages", () => SelectLevelForTownhallAndStorages());
            foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
            {
                RegisterButton(buildingType.ToString(), () => SelectLevelForBuilding(buildingType));
            }
            RegisterBackButton("Cheats", () => Start());
            RegisterTownButton(isDoubleBack: true);

            await SendDialogMessage("Buildings".Bold(), GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task SelectLevelForTownhallAndStorages()
        {
            ClearButtons();
            var building = BuildingType.TownHall.GetBuilding();
            var maxLevel = building.buildingData.levels.Count;
            for (byte i = 0; i <= maxLevel; i++)
            {
                var levelForDelegate = i; // important!
                RegisterButton($"TownHall: Lvl {i}", () => SetLevelForTownhallAndStorages(levelForDelegate));
            }
            RegisterBackButton("Buildings", () => ShowBuildingsGroup());
            RegisterDoubleBackButton("Cheats", () => Start());

            var text = $"Townhall + Storages | Change level:";
            await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task SelectLevelForBuilding(BuildingType buildingType)
        {
            ClearButtons();
            var building = buildingType.GetBuilding();
            var maxLevel = building.buildingData.levels.Count;
            for (byte i = 0; i <= maxLevel; i++)
            {
                var levelForDelegate = i; // important!
                RegisterButton(i.ToString(), () => SetBuildingLevel(buildingType, levelForDelegate));
            }
            RegisterBackButton("Buildings", () => ShowBuildingsGroup());
            RegisterDoubleBackButton("Cheats", () => Start());

            var text = $"{buildingType} | Change level:";
            await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task SetLevelForTownhallAndStorages(byte townhallLevel)
        {
            var sb = new StringBuilder();
            var buildingsData = session.profile.buildingsData;
            BuildingType.TownHall.GetBuilding().Cheat_SetCurrentLevel(buildingsData, townhallLevel);
            var text = $"{BuildingType.TownHall}:".Bold() + $" setuped level {townhallLevel}";
            sb.AppendLine(text);

            foreach (var building in session.player.buildings.GetBuildingsByCategory(BuildingCategory.Storages))
            {
                byte maxAvailableLevel = 0;
                var buildingLevels = building.buildingData.levels;
                for (byte i = 0; i < buildingLevels.Count; i++)
                {
                    var level = buildingLevels[i];
                    if (level.requiredTownHall <= townhallLevel)
                    {
                        maxAvailableLevel = (byte)(i + 1);
                    }
                }
                building.Cheat_SetCurrentLevel(buildingsData, maxAvailableLevel);
                text = $"{building.buildingType}:".Bold() + $" setuped level {maxAvailableLevel}";
                sb.AppendLine(text);
            }

            await messageSender.SendTextMessage(session.chatId, sb.ToString())
                .ConfigureAwait(false);

            await Start()
                .ConfigureAwait(false);
        }

        private async Task SetBuildingLevel(BuildingType buildingType, byte level)
        {
            var building = buildingType.GetBuilding();
            building.Cheat_SetCurrentLevel(session.profile.buildingsData, level);

            var text = $"{buildingType}:".Bold() + $" setuped level {level}";
            await messageSender.SendTextMessage(session.chatId, text)
                .ConfigureAwait(false);

            await Start()
                .ConfigureAwait(false);
        }

        #endregion

        #region Quest Progress Group

        private async Task ShowQuestProgressGroup()
        {
            ClearButtons();
            foreach (QuestType questType in Enum.GetValues(typeof(QuestType)))
            {
                if (questType == QuestType.None)
                    continue;

                RegisterButton(questType.ToString(), () => SelectQuestStage(questType));
            }
            RegisterBackButton("Cheats", () => Start());
            RegisterTownButton(isDoubleBack: true);

            var sb = new StringBuilder();
            sb.AppendLine("Quest Progress".Bold());
            
            var focusedQuest = session.profile.dynamicData.quests.GetFocusedQuest();
            if (focusedQuest.HasValue)
            {
                sb.AppendLine();
                var quest = QuestsHolder.GetQuest(focusedQuest.Value);
                sb.AppendLine($"Current quest: {focusedQuest}");
                sb.AppendLine($"Current stage: {quest.GetCurrentStage(session).id}");
            }            

            await SendDialogMessage(sb.ToString(), GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task SelectQuestStage(QuestType questType)
        {
            var quest = QuestsHolder.GetQuest(questType);
            if (quest == null)
                return;

            var stages = quest.stages;
            ClearButtons();
            foreach (var stage in stages)
            {
                var comment = stage.comment.Trim().Replace(Environment.NewLine, " ");
                var stageView = $"{stage.id} - {comment}";
                RegisterButton(stageView, () => SetupCurrentQuestProgress(questType, stage.id));
            }
            RegisterBackButton("Quest Progress", () => ShowQuestProgressGroup());
            RegisterDoubleBackButton("Cheats", () => Start());

            var text = $"{questType} | Set stage:";
            await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task SetupCurrentQuestProgress(QuestType questType, int stageId)
        {
            var quest = QuestsHolder.GetQuest(questType);
            if (quest == null)
                return;

            var sb = new StringBuilder();
            sb.AppendLine($"Quest progress changed".Bold());
            sb.AppendLine($"Current quest: {questType}");
            sb.AppendLine($"Current stage: {stageId}");
            await messageSender.SendTextMessage(session.chatId, sb.ToString())
                .ConfigureAwait(false);

            var playerQuestsProgress = session.profile.dynamicData.quests;
            playerQuestsProgress.Cheat_SetCurrentQuest(questType, stageId);
            await quest.SetStage(session, stageId)
                .ConfigureAwait(false);
        }

        #endregion

    }
}
