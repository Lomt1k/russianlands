using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
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
        centerScreenBlock.Add(HtmlHelper.CreateForm(localPath, page, "Search", new InputFieldInfo("telegramId", "Search by Telegram ID")));
        centerScreenBlock.Add(HtmlHelper.CreateForm(localPath, page, "Search", new InputFieldInfo("username", "Search by @username")));
        centerScreenBlock.Add(HtmlHelper.CreateForm(localPath, page, "Search", new InputFieldInfo("nickname", "Search by game nickname")));

        centerScreenBlock.Add(HtmlHelper.CreateForm(localPath, page, "Search by Telegram name", "Search",
            new InputFieldInfo("firstName", "First Name"),
            new InputFieldInfo("lastName", "Last Name")
            )).AddProperties(new HProp("style", "margin-top: 50px;"));

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
            var document = HtmlHelper.CreateErrorPage("Player Search", "Incorrect Telegram ID", localPath + $"?page={page}");
            response.AsTextUTF8(document.GenerateHTML());
            response.Close();
            return;
        }

        var db = BotController.dataBase.db;
        var query = db.Table<ProfileData>().Where(x => x.telegram_id == longTelegramId);
        var profileData = await query.FirstOrDefaultAsync().FastAwait();
        if (profileData == null)
        {
            var document = HtmlHelper.CreateErrorPage("Player Search", $"Player with TelegramID {longTelegramId} not found", localPath + $"?page={page}");
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
            var document = HtmlHelper.CreateErrorPage("Player Search", $"Player with username @{username} not found", localPath + $"?page={page}");
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
            var document = HtmlHelper.CreateErrorPage("Player Search", $"Player with nickname '{nickname}' not found", localPath + $"?page={page}");
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
            var document = HtmlHelper.CreateErrorPage("Player Search", $"Player with name '{name}' not found", localPath + $"?page={page}");
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

    private async Task ShowProfile(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, string localPath, ProfileData profileDate, bool fromActivePlayers = false)
    {
        var document = HtmlHelper.CreateDocument($"[{profileDate.telegram_id}] {profileDate.nickname}");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent(), new HProp("align", "center"));

        var session = sessionManager.GetSessionIfExists(profileDate.telegram_id);
        var isOnline = session is not null;

        var header = profileDate.nickname + (isOnline ? " <i>(Online)</i>" : string.Empty);
        var generalInfo = GetGeneralProfileInfo(session?.profile.data ?? profileDate);
        var resourceInfo = GetProfileResourcesInfo(session?.profile.data ?? profileDate);

        var div = new HTag("div", StylesHelper.CenterScreenBlock(700, 900), new HProp("align", "center"))
        {
            { "h1", header },
            generalInfo,
            resourceInfo,
            HtmlHelper.CreateLinkButton("<< Back", localPath + $"?page={page}" + (fromActivePlayers ? "&showActivePlayers=" : string.Empty) ),
        };
        document["html"]["body"].Add(div);
        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private HTag GetGeneralProfileInfo(ProfileData profileData)
    {
        var endPremiumDt = new DateTime(profileData.endPremiumTime);
        var premiumValue = profileData.IsPremiumActive() ? $"ACTIVE (until {endPremiumDt.ToLongDateString()})"
            : profileData.IsPremiumExpired() ? $"EXPIRED {endPremiumDt.ToLongDateString()}"
            : "NEVER BUY";

        var table = new HTag("table", new HProp("frame", "hsides"), new HProp("align", "left"))
        {
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
        };
        return table;
    }

    private HTag GetProfileResourcesInfo(ProfileData profileData)
    {
        var table = new HTag("table", new HProp("frame", "hsides"), new HProp("align", "center"));
        foreach (var resourceId in Enum.GetValues<ResourceId>())
        {
            if (resourceId == ResourceId.InventoryItems)
            {
                continue;
            }

            var localizationKey = "resource_name_" + resourceId.ToString().ToLower();
            var amount = ResourcesDictionary.Get(resourceId).GetValue(profileData);
            var resourceView = resourceId.GetEmoji() + (Localization.Get(LanguageCode.EN, localizationKey) + ':').Bold() + $" {amount.View()}";
            table.Add(CreateTableRow(resourceView));
        }
        return table;
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
            row.AddChild(record);
        }
        return row;
    }

}
