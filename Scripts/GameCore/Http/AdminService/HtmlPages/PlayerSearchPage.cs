﻿using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Skills;
using Obisoft.HSharp.Models;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages;
internal class PlayerSearchPage : IHtmlPage
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();

    public string page => "playerSearch";

    public async Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var showActivePlayers = query["showActivePlayers"];
        var telegramId = query["telegramId"];
        if (telegramId is not null)
        {
            var fromActivePlayers = showActivePlayers is not null;
            await SearchByTelegramId(response, sessionInfo, localPath, telegramId, fromActivePlayers).FastAwait();
            return;
        }        
        if (showActivePlayers is not null)
        {
            ShowActivePlayers(response, sessionInfo, localPath);
            return;
        }
        
        var username = query["username"];
        if (username is not null)
        {
            await SearchByUsername(response, sessionInfo, localPath, username).FastAwait();
            return;
        }
        var nickname = query["nickname"];
        if (nickname is not null)
        {
            await SearchByNickname(response, sessionInfo, localPath, nickname).FastAwait();
            return;
        }
        var firstName = query["firstName"];
        var lastName = query["lastName"];
        if (firstName is not null && lastName is not null)
        {
            await SearchByFirstAndLastName(response, sessionInfo, localPath, firstName, lastName).FastAwait();
            return;
        }


        ShowDefaultSearchPage(response, localPath);
    }

    private void ShowDefaultSearchPage(HttpListenerResponse response, string localPath)
    {
        // prepare document
        var document = HtmlHelper.CreateDocument("Player Search");

        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());
        var centerScreenBlock = new HTag("div", new HProp("align", "center"), StylesHelper.CenterScreenBlock(700, 700));
        document["html"]["body"].Add(centerScreenBlock);

        // seach
        centerScreenBlock.Add(HtmlHelper.CreateSimpleForm(localPath, page, "Search", "telegramId", "Search by Telegram ID"));
        centerScreenBlock.Add(HtmlHelper.CreateSimpleForm(localPath, page, "Search", "username", "Search by @username"));
        centerScreenBlock.Add(HtmlHelper.CreateSimpleForm(localPath, page, "Search", "nickname", "Search by game nickname"));

        var searchByNameForm = new HtmlFormBuilder(localPath)
            .AddHeader("Search by Telegram name")
            .AddHiddenInput("page", page)
            .AddInput("firstName", "First Name")
            .AddInput("lastName", "Last Name")
            .AddButton("Search")
            .GetResult();
        searchByNameForm.AddProperties(new HProp("style", "margin-top: 50px;"));
        centerScreenBlock.Add(searchByNameForm);

        var bottomPanel = new HTag("div", new HProp("align", "center"), new HProp("style", "margin-top: 100px;"))
        {
            HtmlHelper.CreateLinkButton("<< Back", localPath)
        };
        centerScreenBlock.Add(bottomPanel);

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private void ShowActivePlayers(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, string localPath)
    {
        var profileDatas = sessionManager.GetAllSessions().Select(x => x.profile.data).ToArray();
        ShowProfilesList(response, localPath, profileDatas, showActivePlayers: true);
    }

    private async Task SearchByTelegramId(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, string localPath, string telegramId, bool fromActivePlayers)
    {
        if (!long.TryParse(telegramId, out var longTelegramId))
        {
            var document = HtmlHelper.CreateMessagePage("Player Search", "Incorrect Telegram ID", localPath + $"?page={page}");
            response.AsTextUTF8(document.GenerateHTML());
            response.Close();
            return;
        }

        var db = BotController.dataBase.db;
        var query = db.Table<ProfileData>().Where(x => x.telegram_id == longTelegramId);
        var profileData = await query.FirstOrDefaultAsync().FastAwait();
        if (profileData == null)
        {
            var document = HtmlHelper.CreateMessagePage("Player Search", $"Player with TelegramID {longTelegramId} not found", localPath + $"?page={page}");
            response.AsTextUTF8(document.GenerateHTML());
            response.Close();
            return;
        }

        await ShowProfile(response, sessionInfo, localPath, profileData, fromActivePlayers).FastAwait();
    }

    private async Task SearchByUsername(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, string localPath, string username)
    {
        username = username.TrimStart('@');

        var db = BotController.dataBase.db;
        var query = db.Table<ProfileData>().Where(x => x.username == username);
        var profileData = await query.FirstOrDefaultAsync().FastAwait();
        if (profileData == null)
        {
            var document = HtmlHelper.CreateMessagePage("Player Search", $"Player with username @{username} not found", localPath + $"?page={page}");
            response.AsTextUTF8(document.GenerateHTML());
            response.Close();
            return;
        }

        await ShowProfile(response, sessionInfo, localPath, profileData).FastAwait();
    }

    private async Task SearchByNickname(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, string localPath, string nickname)
    {
        var db = BotController.dataBase.db;
        var query = db.Table<ProfileData>().Where(x => x.nickname.Contains(nickname));
        var profileDatas  = await query.Take(100).ToArrayAsync().FastAwait();
        if (profileDatas.Length < 1)
        {
            var document = HtmlHelper.CreateMessagePage("Player Search", $"Player with nickname '{nickname}' not found", localPath + $"?page={page}");
            response.AsTextUTF8(document.GenerateHTML());
            response.Close();
            return;
        }
        if (profileDatas.Length == 1)
        {
            var profile = profileDatas[0];
            await ShowProfile(response, sessionInfo, localPath, profile).FastAwait();
            return;
        }
        ShowProfilesList(response, localPath, profileDatas);
    }

    private async Task SearchByFirstAndLastName(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, string localPath, string firstName, string lastName)
    {
        var db = BotController.dataBase.db;
        var query = firstName.Length > 0 && lastName.Length > 0
            ? db.Table<ProfileData>().Where(x => x.lastName != null && x.firstName.Contains(firstName) && x.lastName.Contains(lastName))
            : lastName.Length > 0 ? db.Table<ProfileData>().Where(x => x.lastName != null && x.lastName.Contains(lastName))
            : db.Table<ProfileData>().Where(x => x.firstName.Contains(firstName));
        var profileDatas = await query.Take(100).ToArrayAsync().FastAwait();

        if (profileDatas.Length < 1)
        {
            var name = (string.IsNullOrEmpty(firstName) ? string.Empty : firstName) + ' ' + (string.IsNullOrEmpty(lastName) ? string.Empty : lastName);
            var document = HtmlHelper.CreateMessagePage("Player Search", $"Player with name '{name}' not found", localPath + $"?page={page}");
            response.AsTextUTF8(document.GenerateHTML());
            response.Close();
            return;
        }

        if (profileDatas.Length == 1)
        {
            var profile = profileDatas[0];
            await ShowProfile(response, sessionInfo, localPath, profile).FastAwait();
            return;
        }
        ShowProfilesList(response, localPath, profileDatas);
    }

    private void ShowProfilesList(HttpListenerResponse response, string localPath, ProfileData[] profileDatas, bool showActivePlayers = false)
    {
        var document = HtmlHelper.CreateDocument($"Profiles ({profileDatas.Length})");

        var table = document["html"]["body"].Add("table", new HProp("cellpadding", "10"), new HProp("style", "border-collapse: collapse;"));
        var headers = table.Add("tr", new HProp("style", "height: 50px; border-bottom: 1pt solid black; background-color: #DDDDDD"));
        headers.Add("th", "Telegram ID", new HProp("align", "left"));
        headers.Add("th", "Nick", new HProp("align", "left"));
        headers.Add("th", "Level", new HProp("align", "left"));
        headers.Add("th", "First Name", new HProp("align", "left"));
        headers.Add("th", "Last Name", new HProp("align", "left"));
        headers.Add("th", "Username", new HProp("align", "left"));
        headers.Add("th", "Last Active", new HProp("align", "left"));
        headers.Add("th", "&nbsp;");

        foreach ( var profileData in profileDatas )
        {
            var row = table.Add("tr", new HProp("style", "height: 50px; border-bottom: 1pt solid black;"));
            row.Add("td", $"ID {profileData.telegram_id}");
            row.Add("td").Add("b", profileData.nickname);
            row.Add("td", profileData.level.ToString());
            row.Add("td", profileData.firstName);
            row.Add("td", profileData.lastName ?? string.Empty);
            row.Add("td", profileData.username ?? string.Empty);
            row.Add("td", profileData.lastActivityTime);
            row.Add("td").Add(HtmlHelper.CreateLinkButton("View", $"{localPath}?page={page}&telegramId={profileData.telegram_id}"
                + (showActivePlayers ? "&showActivePlayers=" : string.Empty), size: 14));
        }

        var bottomPanel = new HTag("div", new HProp("style", "margin-left: 300px; margin-top: 40px;"))
        {
            HtmlHelper.CreateLinkButton("<< Back", localPath + (!showActivePlayers ? $"?page={page}" : string.Empty))
        };
        document["html"]["body"].Add(bottomPanel);

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task ShowProfile(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, string localPath, ProfileData profileData, bool fromActivePlayers = false)
    {
        var document = HtmlHelper.CreateDocument($"[{profileData.telegram_id}] {profileData.nickname}");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent(), new HProp("align", "center"));

        var session = sessionManager.GetSessionIfExists(profileData.telegram_id);
        var isOnline = session is not null;
        var db = BotController.dataBase.db;

        var firstTable = GetGeneralProfileInfo(session?.profile.data ?? profileData, isOnline);
        var equippedItemsInfo = await GetEquippedItemsInfo(session, sessionInfo, profileData).FastAwait();
        firstTable.Add(equippedItemsInfo);

        var secondTable = GetProfileResourcesInfo(session?.profile.data ?? profileData, sessionInfo);

        var buildingsData = session?.profile.buildingsData ?? await db.Table<ProfileBuildingsData>().Where(x => x.dbid == profileData.dbid).FirstAsync().FastAwait();
        var thirdTable = GetProfileBuildingsInfo(buildingsData, sessionInfo);
        var skillsInfo = GetProfileSkillsInfo(profileData, sessionInfo);
        thirdTable.Add(skillsInfo);

        var centerBlock = new HTag("div", StylesHelper.CenterScreenBlock(1000, 900), new HProp("align", "center"))
        {
            new HTag("div", new HProp("align", "left"))
            {
                firstTable,
                secondTable,
                thirdTable,
            },
            new HTag("div", new HProp("align", "center"), new HProp("style", "clear: both; margin-top:60px;"))
            {
                HtmlHelper.CreateLinkButton("<< Back", localPath + $"?page={page}" + (fromActivePlayers ? "&showActivePlayers=" : string.Empty) ),
                HtmlHelper.CreateLinkButton("Last Logs", localPath + $"?page=showLog&mode=search&searchId={profileData.telegram_id}" + (fromActivePlayers ? "&showActivePlayers=" : string.Empty), color: "#808080"),
                HtmlHelper.CreateLinkButton("Add Resource", localPath + $"?page=addResource&telegramId={profileData.telegram_id}" + (fromActivePlayers ? "&showActivePlayers=" : string.Empty), color: "#808080"),
                HtmlHelper.CreateLinkButton("Add Item", localPath + $"?page=addItem&telegramId={profileData.telegram_id}" + (fromActivePlayers ? "&showActivePlayers=" : string.Empty), color: "#808080"),
                HtmlHelper.CreateLinkButton("Set Building Level", localPath + $"?page=setBuildingLevel&telegramId={profileData.telegram_id}" + (fromActivePlayers ? "&showActivePlayers=" : string.Empty), color: "#808080"),
            }
        };
        document["html"]["body"].Add(centerBlock);
        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private HTag GetGeneralProfileInfo(ProfileData profileData, bool isOnline)
    {
        var endPremiumDt = new DateTime(profileData.endPremiumTime);
        var premiumValue = profileData.IsPremiumActive() ? $"ACTIVE (until {endPremiumDt.ToLongDateString()})"
            : profileData.IsPremiumExpired() ? $"EXPIRED {endPremiumDt.ToLongDateString()}"
            : "NEVER BUY";

        var generalInfo = new HTag("div", new HProp("style", "float: left; margin-left: 70px;"))
        {
            { "h3", "General Info" },
            new HTag("table", new HProp("frame", "hsides"))
            {
                CreateTableRow("Nickname", profileData.nickname),
                CreateTableRow("Status: ", isOnline ? "<font color=#008D00><b>Online</b></font>" : "Offline"),
                CreateTableRow("Telegram ID", profileData.telegram_id.ToString()),
                CreateTableRow("First Name", profileData.firstName),
                CreateTableRow("Last Name", profileData.lastName ?? string.Empty),
                CreateTableRow("Username", profileData.username ?? string.Empty),
                CreateTableRow("Level", profileData.level.ToString()),

                CreateTableRow("Registration Date", profileData.regDate),
                CreateTableRow("Registration Version", profileData.regVersion),
                CreateTableRow("Last Version", profileData.lastVersion),
                CreateTableRow("Last Activity Time", profileData.lastActivityTime),

                CreateTableRow("Language", profileData.language.ToString()),
                CreateTableRow("Admin Status", profileData.adminStatus.ToString()),
                CreateTableRow("Premium", premiumValue),
            }
        };
        return generalInfo;
    }

    private async Task<HTag> GetEquippedItemsInfo(GameSession? session, HttpAdminSessionInfo sessionInfo, ProfileData profileData)
    {
        InventoryItem[]? equippedItems = null;
        if (session is not null)
        {
            equippedItems = session.player.inventory.equipped.allEquipped;
        }
        else
        {
            var db = BotController.dataBase.db;
            var rawDynamicData = await db.Table<RawProfileDynamicData>().Where(x => x.dbid == profileData.dbid).FirstAsync().FastAwait();
            var dynamicData = ProfileDynamicData.Deserialize(rawDynamicData);
            equippedItems = dynamicData.inventory.equipped.allEquipped;
        }

        var equippedItemsInfo = new HTag("div")
        {
            { "h3", "Equipped Items" },
            { "table", new HProp("frame", "hsides") },
        };
        foreach (var item in equippedItems)
        {
            var row = CreateTableRow(item.GetFullName(sessionInfo.languageCode));
            equippedItemsInfo["table"].Add(row);
        }
        return equippedItemsInfo;
    }

    private HTag GetProfileResourcesInfo(ProfileData profileData, HttpAdminSessionInfo sessionInfo)
    {
        var resourcesInfo = new HTag("div", new HProp("style", "float: left; margin-left:60px; margin-right: 60px;"))
        {
            { "h3", "Resources" },
            { new HTag("table", new HProp("frame", "hsides")) },
        };

        foreach (var resourceId in Enum.GetValues<ResourceId>())
        {
            if (resourceId == ResourceId.InventoryItems)
            {
                continue;
            }

            var localizationKey = "resource_name_" + resourceId.ToString().ToLower();
            var amount = ResourcesDictionary.Get(resourceId).GetValue(profileData);
            var resourceView = resourceId.GetEmoji() + (Localization.Get(sessionInfo.languageCode, localizationKey) + ':').Bold() + $" {amount.View()}";
            resourcesInfo["table"].Add(CreateTableRow(resourceView));
        }
        return resourcesInfo;
    }

    private HTag GetProfileBuildingsInfo(ProfileBuildingsData buildingsData, HttpAdminSessionInfo sessionInfo)
    {
        var buildingsInfo = new HTag("div", new HProp("style", "float: left;"))
        {
            { "h3", "Buildings" },
            { new HTag("table", new HProp("frame", "hsides")) },
        };

        foreach (var buildingId in Enum.GetValues<BuildingId>())
        {
            var building = buildingId.GetBuilding();
            var level = building.GetCurrentLevel(buildingsData);
            if (level < 1)
            {
                continue;
            }
            var buildingName = Emojis.ButtonBuildings + building.GetLocalizedName(sessionInfo.languageCode, buildingsData);
            buildingsInfo["table"].Add(CreateTableRow(buildingName));
        }
        return buildingsInfo;
    }

    private HTag GetProfileSkillsInfo(ProfileData profileData, HttpAdminSessionInfo sessionInfo)
    {
        var skillsInfo = new HTag("div")
        {
            { "h3", "Skills" },
            { new HTag("table", new HProp("frame", "hsides")) },
        };

        foreach (var itemType in SkillsDictionary.GetAllSkillTypes())
        {
            if (!SkillsDictionary.ContainsKey(itemType))
            {
                continue;
            }

            var value = SkillsDictionary.Get(itemType).GetValue(profileData);
            var buildingName = itemType.GetEmoji() + itemType.GetLocalization(sessionInfo.languageCode) + $": {value}";
            skillsInfo["table"].Add(CreateTableRow(buildingName));
        }
        return skillsInfo;
    }

    private HTag CreateTableRow(string header, params string[] args)
    {
        var row = new HTag("tr")
        {
            { "th", header, new HProp("align", "left") }
        };
        foreach (var arg in args)
        {
            var record = new HTag("td", arg);
            record.AddProperties(new HProp("align", "right"));
            row.Add(record);
        }
        return row;
    }

}
