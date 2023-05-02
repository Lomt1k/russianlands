using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InputFiles;
using TextGameRPG.Scripts.Bot.Commands;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.Generators;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Skills;

namespace TextGameRPG.Scripts.Bot.Dialogs.Cheats;

public class CheatsDialog : DialogBase
{
    private static readonly SessionManager sessionManager = Services.Get<SessionManager>();

    private Func<string, Task>? _onReceivedFileFromUser;

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
        RegisterButton("Resources", ShowResourcesGroup);
        RegisterButton("Items", ShowItemsGroup);
        RegisterButton("Buildings", ShowBuildingsGroup);
        RegisterButton("Skills", ShowSkillsGroup);
        RegisterButton("Quest Progress", ShowQuestProgressGroup);
        RegisterButton("Language", ShowLanguageGroup);
        RegisterButton("Account", ShowAccountGroup);
        RegisterTownButton(isDoubleBack: false);

        await SendDialogMessage(sb, GetKeyboardWithRowSizes(3, 2, 2, 1)).FastAwait();
    }

    #region Resources Group

    public async Task ShowResourcesGroup()
    {
        ClearButtons();
        foreach (ResourceId resourceId in Enum.GetValues(typeof(ResourceId)))
        {
            if (resourceId == ResourceId.InventoryItems)
                continue;

            var shortName = resourceId.IsCraftResource()
                ? resourceId.ToString().Replace("Pieces", string.Empty)
                : resourceId.ToString();
            RegisterButton(shortName, () => SelectAmountForAddResource(resourceId));
        }
        RegisterBackButton("Cheats", Start);
        RegisterTownButton(isDoubleBack: true);

        await SendDialogMessage("Resources".Bold(), GetKeyboardWithFixedRowSize(3)).FastAwait();
    }

    public async Task SelectAmountForAddResource(ResourceId resourceId)
    {
        ClearButtons();
        RegisterButton("10", () => InvokeAddResourceCommand(resourceId, 10));
        RegisterButton("50", () => InvokeAddResourceCommand(resourceId, 50));
        RegisterButton("100", () => InvokeAddResourceCommand(resourceId, 100));
        RegisterButton("1k", () => InvokeAddResourceCommand(resourceId, 1_000));
        RegisterButton("10k", () => InvokeAddResourceCommand(resourceId, 10_000));
        RegisterButton("100k", () => InvokeAddResourceCommand(resourceId, 100_000));
        RegisterButton("1kk", () => InvokeAddResourceCommand(resourceId, 1_000_000));
        RegisterButton("10kk", () => InvokeAddResourceCommand(resourceId, 10_000_000));
        RegisterButton("MAX", () => InvokeAddResourceCommand(resourceId, int.MaxValue));
        RegisterBackButton("Resources", ShowResourcesGroup);
        RegisterDoubleBackButton("Cheats", Start);

        var text = $"{resourceId} | Add amount:";
        await SendDialogMessage(text, GetKeyboardWithRowSizes(3, 3, 3, 2)).FastAwait();
    }

    public async Task InvokeAddResourceCommand(ResourceId resourceId, int amount)
    {
        var command = $"/addresource {resourceId} {amount}";
        await messageSender.SendTextMessage(session.chatId, command).FastAwait();
        await CommandHandler.HandleCommand(session, command).FastAwait();
        await Start().FastAwait();
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
        RegisterBackButton("Cheats", Start);

        await SendDialogMessage("Items".Bold(), GetKeyboardWithFixedRowSize(3)).FastAwait();
    }

    public async Task SelectRarityForItem(ItemType itemType)
    {
        ClearButtons();
        foreach (Rarity rarity in Enum.GetValues(typeof(Rarity)))
        {
            RegisterButton(rarity.ToString(), () => SelectTownhallLevelForItem(itemType, rarity));
        }
        RegisterBackButton("ItemType", ShowItemsGroup);
        RegisterDoubleBackButton("Cheats", Start);

        var text = $"{itemType} | Select rarity:";
        await SendDialogMessage(text, GetKeyboardWithFixedRowSize(2)).FastAwait();
    }

    public async Task SelectTownhallLevelForItem(ItemType itemType, Rarity rarity)
    {
        ClearButtons();
        for (var i = 1; i <= 8; i++)
        {
            var levelForDelegate = i;
            RegisterButton(levelForDelegate.ToString(), () => InvokeAddItemCommand(itemType, rarity, levelForDelegate));
        }
        RegisterBackButton("Rarity", () => SelectRarityForItem(itemType));
        RegisterDoubleBackButton("Items", ShowItemsGroup);

        var text = $"{itemType}, {rarity} | Select Townhall:";
        await SendDialogMessage(text, GetKeyboardWithFixedRowSize(4)).FastAwait();
    }

    public async Task InvokeAddItemCommand(ItemType itemType, Rarity rarity, int townhallLevel)
    {
        var item = itemType == ItemType.Any
            ? ItemGenerationManager.GenerateItemWithSmartRandom(session, townhallLevel, rarity)
            : ItemGenerationManager.GenerateItem(townhallLevel, itemType, rarity);
        var command = $"/additem {item.id}";

        ClearButtons();
        RegisterButton(Emojis.ElementPlus + "Generate", () => InvokeAddItemCommand(itemType, rarity, townhallLevel));
        RegisterBackButton("Items", ShowItemsGroup);
        RegisterDoubleBackButton("Cheats", Start);

        await SendDialogMessage(command, GetMultilineKeyboardWithDoubleBack()).FastAwait();
        await CommandHandler.HandleCommand(session, command).FastAwait();
    }


    #endregion

    #region Buildings Group

    private async Task ShowBuildingsGroup()
    {
        ClearButtons();

        RegisterButton("All buildings", SelectLevelForAllBuildings);
        RegisterButton("Townhall + Storages", SelectLevelForTownhallAndStorages);
        foreach (BuildingId buildingId in Enum.GetValues(typeof(BuildingId)))
        {
            RegisterButton(buildingId.ToString(), () => SelectLevelForBuilding(buildingId));
        }
        RegisterBackButton("Cheats", Start);
        RegisterTownButton(isDoubleBack: true);

        await SendDialogMessage("Buildings".Bold(), GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task SelectLevelForTownhallAndStorages()
    {
        ClearButtons();
        var building = BuildingId.TownHall.GetBuilding();
        var maxLevel = building.buildingData.levels.Count;
        for (byte i = 0; i <= maxLevel; i++)
        {
            var levelForDelegate = i; // important!
            RegisterButton($"TownHall: Lvl {i}", () => SetLevelForTownhallAndStorages(levelForDelegate));
        }
        RegisterBackButton("Buildings", ShowBuildingsGroup);
        RegisterDoubleBackButton("Cheats", Start);

        var text = $"Townhall + Storages | Change level:";
        await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task SelectLevelForAllBuildings()
    {
        ClearButtons();
        var building = BuildingId.TownHall.GetBuilding();
        var maxLevel = building.buildingData.levels.Count;
        for (byte i = 0; i <= maxLevel; i++)
        {
            var levelForDelegate = i; // important!
            RegisterButton($"TownHall: Lvl {i}", () => SetLevelForAllBuildingsByTownhall(levelForDelegate));
        }
        RegisterBackButton("Buildings", ShowBuildingsGroup);
        RegisterDoubleBackButton("Cheats", Start);

        var text = $"All buildings | Set level by townhall:";
        await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task SelectLevelForBuilding(BuildingId buildingId)
    {
        ClearButtons();
        var building = buildingId.GetBuilding();
        var maxLevel = building.buildingData.levels.Count;
        for (byte i = 0; i <= maxLevel; i++)
        {
            var levelForDelegate = i; // important!
            RegisterButton(i.ToString(), () => SetBuildingLevel(buildingId, levelForDelegate));
        }
        RegisterBackButton("Buildings", ShowBuildingsGroup);
        RegisterDoubleBackButton("Cheats", Start);

        var text = $"{buildingId} | Change level:";
        await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task SetLevelForTownhallAndStorages(byte townhallLevel)
    {
        var sb = new StringBuilder();
        var buildingsData = session.profile.buildingsData;
        BuildingId.TownHall.GetBuilding().Cheat_SetCurrentLevel(buildingsData, townhallLevel);
        var text = $"{BuildingId.TownHall}:".Bold() + $" setuped level {townhallLevel}";
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
            text = $"{building.buildingId}:".Bold() + $" setuped level {maxAvailableLevel}";
            sb.AppendLine(text);
        }

        await messageSender.SendTextMessage(session.chatId, sb.ToString()).FastAwait();

        await Start().FastAwait();
    }

    private async Task SetLevelForAllBuildingsByTownhall(byte townhallLevel)
    {
        var sb = new StringBuilder();
        var buildingsData = session.profile.buildingsData;
        BuildingId.TownHall.GetBuilding().Cheat_SetCurrentLevel(buildingsData, townhallLevel);
        var text = $"{BuildingId.TownHall}:".Bold() + $" setuped level {townhallLevel}";
        sb.AppendLine(text);

        foreach (var building in session.player.buildings.GetAllBuildings())
        {
            if (building.buildingId == BuildingId.TownHall)
                continue;

            byte maxAvailableLevel = 0;
            var buildingLevels = building.buildingData.levels;
            var minRequiredLevel = 0; // Бывают здания, где у следующих уровней указан требуемый уровень ниже, чем в предыдущим(шахта 2, шахта 3 и тд)
            for (byte i = 0; i < buildingLevels.Count; i++)
            {
                var level = buildingLevels[i];
                minRequiredLevel = level.requiredTownHall > minRequiredLevel ? level.requiredTownHall : minRequiredLevel;
                if (minRequiredLevel <= townhallLevel)
                {
                    maxAvailableLevel = (byte)(i + 1);
                }
            }
            building.Cheat_SetCurrentLevel(buildingsData, maxAvailableLevel);
            text = $"{building.buildingId}:".Bold() + $" setuped level {maxAvailableLevel}";
            sb.AppendLine(text);
        }

        await messageSender.SendTextMessage(session.chatId, sb.ToString()).FastAwait();

        await Start().FastAwait();
    }

    private async Task SetBuildingLevel(BuildingId buildingId, byte level)
    {
        var building = buildingId.GetBuilding();
        building.Cheat_SetCurrentLevel(session.profile.buildingsData, level);

        var text = $"{buildingId}:".Bold() + $" setuped level {level}";
        await messageSender.SendTextMessage(session.chatId, text).FastAwait();
        await Start().FastAwait();
    }

    #endregion

    #region Skills Group

    private async Task ShowSkillsGroup()
    {
        ClearButtons();

        foreach (var itemType in PlayerSkills.GetAllSkillTypes())
        {
            RegisterButton(itemType.ToString(), () => SelectLevelForSkill(itemType));
        }
        RegisterButton("ALL", () => SelectLevelForSkill(ItemType.Any));
        RegisterBackButton("Cheats", Start);
        RegisterTownButton(isDoubleBack: true);

        var text = "Skills\n\n".Bold() + session.player.skills.GetShortView();
        await SendDialogMessage(text, GetKeyboardWithRowSizes(3, 3, 3, 2)).FastAwait();
    }

    private async Task SelectLevelForSkill(ItemType itemType)
    {
        ClearButtons();
        var elixirWorkshop = (ElixirWorkshopBuilding)BuildingId.ElixirWorkshop.GetBuilding();
        var buildingLevels = elixirWorkshop.buildingData.levels;
        var maxLevel = ((ElixirWorkshopLevelInfo)buildingLevels[buildingLevels.Count - 1]).skillLevelLimit;
        for (byte i = 0; i <= maxLevel; i += 5)
        {
            var levelForDelegate = i; // important!
            RegisterButton(i.ToString(), () => SetSkillLevel(itemType, levelForDelegate));
        }
        RegisterBackButton("Skills", ShowSkillsGroup);
        RegisterDoubleBackButton("Cheats", Start);

        var text = itemType == ItemType.Any
            ? "ALL SKILLS | Change skill level:"
            : itemType.GetEmoji() + itemType.GetCategoryLocalization(session) + " | Change skill level:";
        await SendDialogMessage(text, GetKeyboardWithFixedRowSize(5)).FastAwait();
    }

    private async Task SetSkillLevel(ItemType itemType, byte level)
    {
        if (itemType == ItemType.Any)
        {
            foreach (var skillType in PlayerSkills.GetAllSkillTypes())
            {
                session.player.skills.SetValue(skillType, level);
            }
        }
        else
        {
            session.player.skills.SetValue(itemType, level);
        }

        var text = itemType == ItemType.Any
            ? "ALL SKILLS | Level changed to " + level
            : itemType.GetEmoji() + itemType.GetCategoryLocalization(session) + ": skill level changed to " + level;
        await messageSender.SendTextMessage(session.chatId, text).FastAwait();
        await ShowSkillsGroup().FastAwait();
    }

    #endregion

    #region Quest Progress Group

    private async Task ShowQuestProgressGroup()
    {
        ClearButtons();
        foreach (QuestId questId in Enum.GetValues(typeof(QuestId)))
        {
            if (questId == QuestId.None)
                continue;

            RegisterButton(questId.ToString(), () => SelectQuestStage(questId));
        }
        RegisterBackButton("Cheats", Start);
        RegisterTownButton(isDoubleBack: true);

        var sb = new StringBuilder();
        sb.AppendLine("Quest Progress".Bold());

        var focusedQuest = session.profile.dynamicData.quests.GetFocusedQuest();
        if (focusedQuest.HasValue)
        {
            sb.AppendLine();
            var quest = gameDataHolder.quests[focusedQuest.Value];
            sb.AppendLine($"Current quest: {focusedQuest}");
            sb.AppendLine($"Current stage: {quest.GetCurrentStage(session).id}");
        }

        await SendDialogMessage(sb.ToString(), GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task SelectQuestStage(QuestId questId)
    {
        var quest = gameDataHolder.quests[questId];
        var stages = quest.stages;
        ClearButtons();
        foreach (var stage in stages)
        {
            var comment = stage.comment.Trim().Replace(Environment.NewLine, " ");
            var stageView = $"{stage.id} - {comment}";
            RegisterButton(stageView, () => SetupCurrentQuestProgress(questId, stage.id));
        }
        RegisterBackButton("Quest Progress", ShowQuestProgressGroup);
        RegisterDoubleBackButton("Cheats", Start);

        var text = $"{questId} | Set stage:";
        await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task SetupCurrentQuestProgress(QuestId questId, int stageId)
    {
        var quest = gameDataHolder.quests[questId];
        var sb = new StringBuilder();
        sb.AppendLine($"Quest progress changed".Bold());
        sb.AppendLine($"Current quest: {questId}");
        sb.AppendLine($"Current stage: {stageId}");
        await messageSender.SendTextMessage(session.chatId, sb.ToString()).FastAwait();

        var playerQuestsProgress = session.profile.dynamicData.quests;
        playerQuestsProgress.Cheat_SetCurrentQuest(questId, stageId);
        await quest.SetStage(session, stageId).FastAwait();
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
        RegisterBackButton("Cheats", Start);
        RegisterTownButton(isDoubleBack: true);

        var text = "Switch language".Bold() + $"\n\nCurrent language: {session.language}";
        await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    public async Task InvokeLanguageCommand(LanguageCode code)
    {
        var command = $"/language {code}";
        await messageSender.SendTextMessage(session.chatId, command).FastAwait();
        await CommandHandler.HandleCommand(session, command).FastAwait();
        await Start().FastAwait();
    }

    #endregion

    #region Account Group

    private async Task ShowAccountGroup()
    {
        ClearButtons();
        RegisterButton(Emojis.ElementWarning + "Reset game", ResetAccountConfirmation);
        RegisterButton("Export", ExportAccount);
        RegisterButton("Import", ImportAccount);
        RegisterBackButton("Cheats", Start);
        RegisterTownButton(isDoubleBack: true);

        var text = "Account".Bold();
        await SendDialogMessage(text, GetKeyboardWithRowSizes(1, 2, 2)).FastAwait();
    }

    private async Task ResetAccountConfirmation()
    {
        ClearButtons();
        RegisterButton(Emojis.ElementWarning + "Yes, reset!", ResetAccount);
        RegisterBackButton("Account", ShowAccountGroup);
        RegisterDoubleBackButton("Cheats", Start);

        var sb = new StringBuilder();
        sb.AppendLine("Are you sure you want to reset your progress?");
        sb.AppendLine();
        sb.AppendLine($"Nick: ".Bold() + session.player.nickname);
        sb.AppendLine("Database Id: ".Bold() + session.profile.data.dbid);
        sb.AppendLine("Telegram Id: ".Bold() + session.profile.data.telegram_id);
        await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task ResetAccount()
    {
        var telegramId = session.actualUser.Id;
        await ResetAccountInDatabase().FastAwait();
        await messageSender.SendTextDialog(telegramId, "Account has been reseted", "Restart").FastAwait();
    }

    private async Task ResetAccountInDatabase()
    {
        await session.profile.Cheat_ResetProfile().FastAwait();
        await sessionManager.CloseSession(session.chatId).FastAwait();
    }

    private async Task ExportAccount()
    {
        var profileState = ProfileState.Create(session.profile);
        var encryptedData = ProfileStateConverter.Serialize(profileState);
        var fileName = $"{profileState.nickname}_{DateTime.UtcNow.ToShortDateString()}_{profileState.telegramId}.dat";
        var filePath = Path.Combine(Program.cacheDirectory, fileName);

        File.WriteAllText(filePath, encryptedData, Encoding.UTF8);
        using (var stream = File.Open(filePath, FileMode.Open))
        {
            var inputOnlineFile = new InputOnlineFile(stream, fileName);
            await messageSender.SendDocument(session.chatId, inputOnlineFile);
        }
        File.Delete(filePath);
    }

    private async Task ImportAccount()
    {
        ClearButtons();
        RegisterBackButton("Account", ShowAccountGroup);
        RegisterDoubleBackButton("Cheats", Start);

        _onReceivedFileFromUser = OnAccountStateDownloaded;

        await SendDialogMessage("Send profile .dat file", GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task OnAccountStateDownloaded(string filePath)
    {
        var encryptedData = File.ReadAllText(filePath);
        var profileState = ProfileStateConverter.Deserialize(encryptedData);

        if (profileState == null)
        {
            await messageSender.SendTextDialog(session.chatId, "Broken profile data").FastAwait();
            return;
        }

        Program.logger.Debug($"profileState: \n{profileState.nickname} | v{profileState.lastVersion}");

        var realTelegramId = session.actualUser.Id;
        var telegramId = session.profile.data.telegram_id;
        var dbid = session.profile.data.dbid;
        await ResetAccountInDatabase().FastAwait();
        await profileState.ExecuteQuerries(dbid).FastAwait();

        var sb = new StringBuilder();
        sb.AppendLine("Account has been imported");
        sb.AppendLine();
        sb.AppendLine("Account data: ".Preformatted());
        sb.AppendLine("Env: ".Bold() + profileState.environment);
        sb.AppendLine("Nickname: ".Bold() + profileState.nickname);
        sb.AppendLine("Database Id: ".Bold() + profileState.databaseId);
        sb.AppendLine("Telegram Id: ".Bold() + profileState.telegramId);
        sb.AppendLine("Last Version: ".Bold() + profileState.lastVersion);
        sb.AppendLine("Last Date: ".Bold() + profileState.lastDate);
        sb.AppendLine();
        sb.AppendLine("Applied for: ".Preformatted());
        sb.AppendLine("Database Id: ".Bold() + dbid);
        sb.AppendLine("Telegram Id: ".Bold() + telegramId);
        _onReceivedFileFromUser = null;

        await messageSender.SendTextDialog(realTelegramId, sb.ToString(), "Restart").FastAwait();
    }

    #endregion

    public override async Task HandleMessage(Telegram.Bot.Types.Message message)
    {
        if (message.Document != null && _onReceivedFileFromUser != null)
        {
            var fileId = message.Document.FileId;
            var file = await messageSender.GetFileAsync(fileId).FastAwait();
            if (file != null && !string.IsNullOrEmpty(file.FilePath))
            {
                var localPath = Path.Combine(Program.cacheDirectory, fileId);
                using (var fileStream = new FileStream(localPath, FileMode.Create))
                {
                    await messageSender.DownloadFileAsync(file.FilePath, fileStream).FastAwait();
                    fileStream.Close();
                }
                await _onReceivedFileFromUser.Invoke(localPath).FastAwait();
            }
            return;
        }

        await base.HandleMessage(message).FastAwait();
    }


}
