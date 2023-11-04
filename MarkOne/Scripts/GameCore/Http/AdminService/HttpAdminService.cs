using FastTelegramBot.DataTypes;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages;
using MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages.PlayerControl;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using Obisoft.HSharp.Models;
using SimpleHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot.Extensions.LoginWidget;
using static MarkOne.Scripts.Bot.BotConfig;
using Authorization = Telegram.Bot.Extensions.LoginWidget.Authorization;

namespace MarkOne.Scripts.GameCore.Http.AdminService;
public class HttpAdminService : IHttpService
{
    private const int SESSION_TIMEOUT = 36_000;

    private Dictionary<string, HttpAdminSessionInfo> _sessions = new();
    private Dictionary<string, IHtmlPage> _htmlPages = new();
    private IHtmlPage _mainPage;
    private LoginWidget _loginWidget;
    private AdminServiceSettings _settings;
    private string _url;
    private string _localPath;
    private bool _withoutLogin;


    public HttpAdminService(string url, AdminServiceSettings settings)
    {
        _url = url;
        _localPath = settings.localPath;
        _loginWidget = new LoginWidget(BotController.config.token);
        _settings = settings;
        _withoutLogin = settings.withoutLoginOnLocalhost && BotController.config.httpListenerSettings.externalHttpPrefix.Contains("//localhost");

        RegisterHtmlPages();
    }

    private void RegisterHtmlPages()
    {
        _mainPage = new MainAdminPage();
        RegisterPage(_mainPage);
        RegisterPage(new ShowLogPage());
        RegisterPage(new PlayerSearchPage());

        // player control
        RegisterPage(new AddResourcePage());
        RegisterPage(new AddPremiumPage());
        RegisterPage(new AddItemPage());
        RegisterPage(new SetBuildingLevelPage());
        RegisterPage(new SetSkillLevelPage());
        RegisterPage(new SetAdminStatusPage());

        RegisterPage(new NewsEditorPage());
    }

    private void RegisterPage(IHtmlPage page)
    {
        _htmlPages.Add(page.page, page);
    }

    public async Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var sessionId = await CheckLoginStatus(request, response).FastAwait();
        if (sessionId is null)
        {
            if (response == null)
            {
                return;
            }
            response.AsText(GetLoginPage());
            response.Close();
            return;
        }

        var sessionInfo = _sessions[sessionId];
        var query = request.QueryString;
        var page = query["page"];
        if (page is not null && _htmlPages.TryGetValue(page, out var htmlPage))
        {
            await htmlPage.ShowPage(response, sessionInfo, query, _localPath).FastAwait();
            return;
        }
        await _mainPage.ShowPage(response, sessionInfo, query, _localPath).FastAwait();
    }

    private async Task<string?> CheckLoginStatus(HttpListenerRequest request, HttpListenerResponse response)
    {
        var sessionIdFromCookie = request.Cookies["x_admin_session"]?.Value;
        if (sessionIdFromCookie is not null)
        {
            if (_sessions.TryGetValue(sessionIdFromCookie, out var session))
            {
                var lifeTime = DateTime.UtcNow - session.lastUpdateTime;
                if (lifeTime.TotalSeconds < SESSION_TIMEOUT)
                {
                    session.lastUpdateTime = DateTime.UtcNow;
                    return sessionIdFromCookie;
                }
                _sessions.Remove(sessionIdFromCookie);
            }
        }

        var query = request.QueryString;
        var checkHash = query["hash"];

        // check telegram login
        if (checkHash is not null)
        {
            var info = GetInfoForCheckLogin(request);
            var success = _loginWidget.CheckAuthorization(info) == Authorization.Valid;
            if (!success)
            {
                response.AsTextUTF8("<h3>Telegram login fail.</h3>");
                response.StatusCode = 403;
                response.Close();
                return null;
            }

            var telegramId = query["id"];
            if (telegramId is null)
            {
                return null;
            }
            var longTelegramId = long.Parse(telegramId);
            var adminStatus = await GetActualAdminStatus(longTelegramId).FastAwait();
            if (adminStatus == AdminStatus.None)
            {
                response.AsTextUTF8("<h3>You dont have admin rights</h3>");
                response.StatusCode = 403;
                response.Close();
                return null;
            }

            var adminData = await GetAdminData(longTelegramId).FastAwait();

            var sessionId = Guid.NewGuid().ToString();
            var sessionInfo = new HttpAdminSessionInfo(longTelegramId, adminData.languageCode, adminData.user);
            _sessions.Add(sessionId, sessionInfo);
            response.SetCookie(new Cookie("x_admin_session", sessionId));
            return sessionId;
        }

        if (_withoutLogin)
        {
            var sessionId = Guid.NewGuid().ToString();
            var sessionInfo = new HttpAdminSessionInfo(-1, BotController.config.defaultLanguageCode, new User() { Id = -1, FirstName = "Unknown" });
            _sessions.Add(sessionId, sessionInfo);
            response.SetCookie(new Cookie("x_admin_session", sessionId));
            return sessionId;
        }

        return null;
    }

    private Dictionary<string, string> GetInfoForCheckLogin(HttpListenerRequest request)
    {
        var result = new Dictionary<string, string>();
        var query = request.QueryString;
        foreach (var key in query.AllKeys)
        {
            if (key is not null)
            {
                result[key] = query[key];
            }
        }
        return result;

    }

    private string GetLoginPage()
    {
        var botname = BotController.botname;
        var script = $"<script async src=\"https://telegram.org/js/telegram-widget.js?22\" data-telegram-login=\"{botname}\" data-size=\"large\" data-auth-url=\"{_url}\"></script>";

        var document = HtmlHelper.CreateDocument("Admin Page");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());
        var loginBlock = new HTag("div");
        loginBlock.AddProperties(StylesHelper.CenterScreenBlock(300, 300));
        loginBlock.AddChild("h3", "Please login with your telegram");
        loginBlock.AddChild("div", script);
        document.AddChild(loginBlock);

        return document.GenerateHTML();
    }

    private async Task<AdminStatus> GetActualAdminStatus(long telegramId)
    {
        if (_withoutLogin)
        {
            return AdminStatus.Root;
        }

        var db = BotController.dataBase.db;
        var query = db.Table<ProfileData>().Where(x => x.telegram_id == telegramId);
        var profileData = query.FirstOrDefault();
        return profileData is not null ? profileData.adminStatus : AdminStatus.None;
    }


    private record AdminData(LanguageCode languageCode, User user);
    private async Task<AdminData> GetAdminData(long telegramId)
    {
        var db = BotController.dataBase.db;
        var query = db.Table<ProfileData>().Where(x => x.telegram_id == telegramId);
        var profileData = query.FirstOrDefault();
        var languageCode = profileData is not null ? profileData.language : BotController.config.defaultLanguageCode;
        var user = profileData is not null
            ? new User
            {
                Id = profileData.telegram_id,
                FirstName = profileData.firstName,
                LastName = profileData.lastName,
                Username = profileData.username,
            }
            : new User()
            {
                Id = -1,
                FirstName = "Unknown"
            };
        var firstName = profileData is not null ? profileData.firstName : "Unknown";
        return new AdminData(languageCode, user);
    }

}
