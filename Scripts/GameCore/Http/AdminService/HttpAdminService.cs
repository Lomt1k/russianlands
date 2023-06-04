﻿using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
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
    private const int SESSION_TIMEOUT = 3600;

    private Dictionary<string, HttpAdminSessionInfo> _sessionsWithLastActivityTime = new();
    private LoginWidget _loginWidget;
    private AdminServiceSettings _settings;
    private string _url;
    private bool _withoutLogin;


    public HttpAdminService(string url, AdminServiceSettings settings)
    {
        _url = url;
        _loginWidget = new LoginWidget(BotController.config.token);
        _settings = settings;
        _withoutLogin = settings.withoutLoginOnLocalhost && BotController.config.httpListenerSettings.externalHttpPrefix.Contains("//localhost");
    }

    public async Task HandleHttpRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var sessionId = await CheckLoginStatus(request, response).FastAwait();
        if (sessionId is null)
        {
            response.AsText(GetLoginPage());
            response.Close();
            return;
        }

        ShowMainAdminPage(response, sessionId);
    }

    private async Task<string?> CheckLoginStatus(HttpListenerRequest request, HttpListenerResponse response)
    {
        var sessionIdFromCookie = request.Cookies["x_admin_session"]?.Value;
        if (sessionIdFromCookie is not null)
        {
            if (_sessionsWithLastActivityTime.TryGetValue(sessionIdFromCookie, out var session))
            {
                var lifeTime = DateTime.UtcNow - session.lastUpdateTime;
                if (lifeTime.TotalSeconds < SESSION_TIMEOUT)
                {
                    session.lastUpdateTime = DateTime.UtcNow;
                    return sessionIdFromCookie;
                }
                _sessionsWithLastActivityTime.Remove(sessionIdFromCookie);
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
                response.AsText("<h3>Telegram login fail.</h3>");
                response.StatusCode = 403;
                return null;
            }

            var telegramId = query["id"];
            if (telegramId is null)
            {
                return null;
            }
            var longTelegramId = long.Parse(telegramId);
            var adminLevel = await GetActualAdminLevel(longTelegramId).FastAwait();
            if (adminLevel < 1)
            {
                response.AsText("<h3>You dont have admin rights</h3>");
                response.StatusCode = 403;
                response.Close();
                return null;
            }

            var sessionId = Guid.NewGuid().ToString();
            var sessionInfo = new HttpAdminSessionInfo(longTelegramId);
            _sessionsWithLastActivityTime.Add(sessionId, sessionInfo);
            response.SetCookie(new Cookie("x_admin_session", sessionId));
            return sessionId;
        }

        if (_withoutLogin)
        {
            var sessionId = Guid.NewGuid().ToString();
            var sessionInfo = new HttpAdminSessionInfo(-1);
            _sessionsWithLastActivityTime.Add(sessionId, sessionInfo);
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
        return "<h3>Please login with your telegram</h3>"
            + $"<script async src=\"https://telegram.org/js/telegram-widget.js?22\" data-telegram-login=\"{botname}\" data-size=\"large\" data-auth-url=\"{_url}\"></script>";
    }

    private async Task<int> GetActualAdminLevel(long telegramId)
    {
        if (_withoutLogin)
        {
            return 1337;
        }

        var db = BotController.dataBase.db;
        var query = db.Table<ProfileData>().Where(x => x.telegram_id == telegramId);
        var profileData = await query.FirstOrDefaultAsync().FastAwait();
        return profileData is not null ? profileData.adminStatus : 0;
    }

    private void ShowMainAdminPage(HttpListenerResponse response, string sessionId)
    {
        response.AsText("<h3>Successfull login</h3>");
        response.Close();
    }

}
