using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using Obisoft.HSharp.Models;
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

        ShowProfile(response, sessionInfo, localPath, profileData, fromActivePlayers);
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

        ShowProfile(response, sessionInfo, localPath, profileData);
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
            ShowProfile(response, sessionInfo, localPath, profile);
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
            ShowProfile(response, sessionInfo, localPath, profile);
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

        var bottomPanel = new HTag("div", new HProp("style", "margin-left: 300px; margin-top: 40px;"));
        bottomPanel.Add(HtmlHelper.CreateLinkButton("<< Back", localPath + (!showActivePlayers ? $"?page={page}" : string.Empty) ));
        document["html"]["body"].Add(bottomPanel);

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private void ShowProfile(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, string localPath, ProfileData profile, bool fromActivePlayers = false)
    {
        // TODO
        var document = HtmlHelper.CreateDocument($"[{profile.telegram_id}] {profile.nickname}");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());
        var div = new HTag("div", StylesHelper.CenterScreenBlock(700, 250))
            {
                { "h1", $"Found Player {profile.nickname}" },
                HtmlHelper.CreateLinkButton("<< Back", localPath + $"?page={page}" + (fromActivePlayers ? "&showActivePlayers=" : string.Empty) ),
            };
        document["html"]["body"].Add(div);
        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

}
