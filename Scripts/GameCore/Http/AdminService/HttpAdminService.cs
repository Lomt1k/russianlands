using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using SimpleHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot.Extensions.LoginWidget;
using Authorization = Telegram.Bot.Extensions.LoginWidget.Authorization;

namespace MarkOne.Scripts.GameCore.Http.AdminService;
public class HttpAdminService : IHttpService
{
    private const int SESSION_TIMEOUT = 3600;

    private Dictionary<string, HttpAdminSessionInfo> _sessionsWithLastActivityTime = new();
    private LoginWidget _loginWidget;

    public HttpAdminService()
    {
        var token = BotController.config.token;
        _loginWidget = new LoginWidget(token);
    }

    public async Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        bool successLogin = await CheckLoginStatus(request, response).FastAwait();
        if (!successLogin)
        {
            response.AsText(GetLoginPage());
            response.Close();
            return;
        }

        response.AsText("<h3>Successfull login</h3>");
        response.Close();
    }

    private async Task<bool> CheckLoginStatus(HttpListenerRequest request, HttpListenerResponse response)
    {
        var query = request.QueryString;
        var checkHash = query["hash"];

        // check telegram login
        if (checkHash is not null)
        {
            var info = GetInfoForCheckLogin(request);
            var success = _loginWidget.CheckAuthorization(info) == Authorization.Valid;
            if (!success)
            {
                response.AsText("<h3>Telegram login fail.</h3>");
                response.StatusCode = 403;
                return false;
            }

            var telegramId = query["id"];
            if (telegramId is null)
            {
                return false;
            }
            var longTelegramId = long.Parse(telegramId);
            var adminLevel = await GetActualAdminLevel(longTelegramId).FastAwait();
            if (adminLevel < 1)
            {
                response.AsText("<h3>You dont have admin rights</h3>");
                response.StatusCode = 403;
                response.Close();
                return false;
            }

            var sessionId = Guid.NewGuid().ToString();
            var sessionInfo = new HttpAdminSessionInfo(longTelegramId);
            _sessionsWithLastActivityTime.Add(sessionId, sessionInfo);
            response.SetCookie(new Cookie("x_admin_session", sessionId));
            return true;
        }

        var sessionIdFromCookie = request.Cookies["x_admin_session"]?.Value;
        if (sessionIdFromCookie is null)
        {
            return false;
        }
        if (_sessionsWithLastActivityTime.TryGetValue(sessionIdFromCookie, out var session))
        {
            var lifeTime = DateTime.UtcNow - session.lastUpdateTime;
            if (lifeTime.TotalSeconds < SESSION_TIMEOUT)
            {
                session.lastUpdateTime = DateTime.UtcNow;
                return true;
            }
            _sessionsWithLastActivityTime.Remove(sessionIdFromCookie);
        }

        return false;
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
        return "<h3>Please login with your telegram</h3>"
            + "<script async src=\"https://telegram.org/js/telegram-widget.js?22\" data-telegram-login=\"russianlandsbot\" data-size=\"large\" data-auth-url=\"https://dev.russianlands.ru/admin\"></script>";
    }

    private async Task<int> GetActualAdminLevel(long telegramId)
    {
        var db = BotController.dataBase.db;
        var query = db.Table<ProfileData>().Where(x => x.telegram_id == telegramId);
        var profileData = await query.FirstOrDefaultAsync().FastAwait();
        return profileData is not null ? profileData.adminStatus : 0;
    }

}
