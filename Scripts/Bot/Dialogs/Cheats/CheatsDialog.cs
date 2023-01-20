using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InputFiles;
using TextGameRPG.Scripts.Bot.Commands;
using TextGameRPG.Scripts.Bot.DataBase.TablesStructure;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.Generators;
using TextGameRPG.Scripts.GameCore.Localizations;
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
            var sb = new StringBuilder();
            sb.AppendLine("Cheats".Bold());
            sb.AppendLine();
            sb.AppendLine($"Nick: ".Bold() + session.player.nickname);
            sb.AppendLine($"Database Id: ".Bold() + session.profile.data.dbid);
            sb.AppendLine($"Telegram Id: ".Bold() + session.profile.data.telegram_id);

            ClearButtons();
            RegisterButton("Resources", () => ShowResourcesGroup());
            RegisterButton("Items", () => ShowItemsGroup());
            RegisterButton("Buildings", () => ShowBuildingsGroup());
            RegisterButton("Quest Progress", () => ShowQuestProgressGroup());
            RegisterButton("Language", () => ShowLanguageGroup());
            RegisterButton("Account", () => ShowAccountGroup());
            RegisterTownButton(isDoubleBack: false);

            await SendDialogMessage(sb, GetKeyboardWithRowSizes(3, 2, 1, 1))
                .ConfigureAwait(false);
        }

        #region Resources Group

        public async Task ShowResourcesGroup()
        {
            ClearButtons();
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                if (resourceType == ResourceType.InventoryItems)
                    continue;

                var shortName = resourceType.IsCraftResource()
                    ? resourceType.ToString().Replace("Pieces", string.Empty)
                    : resourceType.ToString();
                RegisterButton(shortName, () => SelectAmountForAddResource(resourceType));
            }
            RegisterBackButton("Cheats", () => Start());
            RegisterTownButton(isDoubleBack: true);

            await SendDialogMessage("Resources".Bold(), GetKeyboardWithFixedRowSize(3))
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
            var command = $"/addresource {resourceType} {amount}";
            await messageSender.SendTextMessage(session.chatId, command)
                .ConfigureAwait(false);
            await CommandHandler.HandleCommand(session, command)
                .ConfigureAwait(false);
            await Start()
                .ConfigureAwait(false);
        }

        #endregion

        #region Items Group

        public async Task ShowItemsGroup()
        {
            ClearButtons();
            foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
            {
                if (itemType == ItemType.Equipped)
                    continue;

                RegisterButton(itemType.ToString(), () => SelectRarityForItem(itemType));
            }
            RegisterBackButton("Cheats", () => Start());

            await SendDialogMessage("Items".Bold(), GetKeyboardWithFixedRowSize(3))
                .ConfigureAwait(false);
        }

        public async Task SelectRarityForItem(ItemType itemType)
        {
            ClearButtons();
            foreach (Rarity rarity in Enum.GetValues(typeof(Rarity)))
            {
                RegisterButton(rarity.ToString(), () => SelectTownhallLevelForItem(itemType, rarity));
            }
            RegisterBackButton("ItemType", () => ShowItemsGroup());
            RegisterDoubleBackButton("Cheats", () => Start());

            var text = $"{itemType} | Select rarity:";
            await SendDialogMessage(text, GetKeyboardWithFixedRowSize(2))
                .ConfigureAwait(false);
        }

        public async Task SelectTownhallLevelForItem(ItemType itemType, Rarity rarity)
        {
            ClearButtons();
            for (int i = 1; i <= 8; i++)
            {
                var levelForDelegate = i;
                RegisterButton(levelForDelegate.ToString(), () => InvokeAddItemCommand(itemType, rarity, levelForDelegate));
            }
            RegisterBackButton("Rarity", () => SelectRarityForItem(itemType));
            RegisterDoubleBackButton("Items", () => ShowItemsGroup());

            var text = $"{itemType}, {rarity} | Select Townhall:";
            await SendDialogMessage(text, GetKeyboardWithFixedRowSize(4))
                .ConfigureAwait(false);
        }

        public async Task InvokeAddItemCommand(ItemType itemType, Rarity rarity, int townhallLevel)
        {
            var item = itemType == ItemType.Any
                ? ItemGenerationManager.GenerateItem(townhallLevel, rarity)
                : ItemGenerationManager.GenerateItem(townhallLevel, itemType, rarity);
            var command = $"/additem {item.id}";

            ClearButtons();
            RegisterButton(Emojis.ElementPlus + "Generate", () => InvokeAddItemCommand(itemType, rarity, townhallLevel));
            RegisterBackButton("Items", () => ShowItemsGroup());
            RegisterDoubleBackButton("Cheats", () => Start());

            await SendDialogMessage(command, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
            await CommandHandler.HandleCommand(session, command)
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

        #region Language Group

        private async Task ShowLanguageGroup()
        {
            ClearButtons();
            foreach (LanguageCode code in Enum.GetValues(typeof(LanguageCode)))
            {
                RegisterButton(code.ToString(), () => InvokeLanguageCommand(code));
            }
            RegisterBackButton("Cheats", () => Start());
            RegisterTownButton(isDoubleBack: true);

            var text = "Switch language".Bold() + $"\n\nCurrent language: {session.language}";
            await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        public async Task InvokeLanguageCommand(LanguageCode code)
        {
            var command = $"/language {code}";
            await messageSender.SendTextMessage(session.chatId, command)
                .ConfigureAwait(false);
            await CommandHandler.HandleCommand(session, command)
                .ConfigureAwait(false);
            await Start()
                .ConfigureAwait(false);
        }

        #endregion

        #region Account Group

        private async Task ShowAccountGroup()
        {
            ClearButtons();
            RegisterButton(Emojis.ElementWarning + "Reset game", () => ResetAccountConfirmation());
            RegisterButton("Export", () => ExportAccount());
            RegisterButton("Import", () => ImportAccount());
            RegisterBackButton("Cheats", () => Start());
            RegisterTownButton(isDoubleBack: true);

            var text = "Account".Bold();
            await SendDialogMessage(text, GetKeyboardWithRowSizes(1, 2, 2))
                .ConfigureAwait(false);
        }

        private async Task ResetAccountConfirmation()
        {
            ClearButtons();
            RegisterButton(Emojis.ElementWarning + "Yes, reset!", () => ResetAccount());
            RegisterBackButton("Account", () => ShowQuestProgressGroup());
            RegisterDoubleBackButton("Cheats", () => Start());

            var sb = new StringBuilder();
            sb.AppendLine("Are you sure you want to reset your progress?");
            sb.AppendLine();
            sb.AppendLine($"Nick: ".Bold() + session.player.nickname);
            sb.AppendLine("Database Id: ".Bold() + session.profile.data.dbid);
            sb.AppendLine("Telegram Id: ".Bold() + session.profile.data.telegram_id);
            await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task ResetAccount()
        {
            var telegramId = session.actualUser.Id;
            await ResetAccountInDatabase()
                .ConfigureAwait(false);
            await messageSender.SendTextDialog(telegramId, "Account has been reseted", "Restart")
                .ConfigureAwait(false);
        }

        private async Task ResetAccountInDatabase()
        {
            var telegramId = session.profile.data.telegram_id;
            var dbId = session.profile.data.dbid;

            var sessionManager = TelegramBot.instance.sessionManager;
            await sessionManager.CloseSession(telegramId)
                .ConfigureAwait(false);

            var profilesTable = TelegramBot.instance.dataBase[Table.Profiles] as ProfilesDataTable;
            await profilesTable.ResetToDefaultValues(dbId)
                .ConfigureAwait(false);

            var profilesDynamicTable = TelegramBot.instance.dataBase[Table.ProfilesDynamic] as ProfilesDynamicDataTable;
            await profilesDynamicTable.ResetToDefaultValues(dbId)
                .ConfigureAwait(false);

            var profileBuildingsTable = TelegramBot.instance.dataBase[Table.ProfileBuildings] as ProfileBuildingsDataTable;
            await profileBuildingsTable.ResetToDefaultValues(dbId)
                .ConfigureAwait(false);
        }

        private async Task ExportAccount()
        {
            var profileState = ProfileState.Create(session.profile);
            var encryptedData = ProfileStateConverter.Serialize(profileState);
            var fileName = $"v{profileState.lastVersion}_{profileState.nickname}_{profileState.telegramId}.dat";

            File.WriteAllText(@"tempFile.dat", encryptedData, Encoding.UTF8);

            // TODO: этот код не работает...
            using (var stream = new FileStream("tempFile.txt", FileMode.CreateNew)
            {
                var streamWriter = new StreamWriter(stream);
                streamWriter.Write(encryptedData.AsMemory());
                streamWriter.Close();
                var inputOnlineFile = new InputOnlineFile(stream, fileName);

                await messageSender.SendDocument(session.chatId, inputOnlineFile)
                    .ConfigureAwait(false);
            }
        }

        private async Task ImportAccount()
        {
            // TODO
        }

        #endregion


    }
}
