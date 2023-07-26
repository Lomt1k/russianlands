using FastTelegramBot.DataTypes;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using MarkOne.Scripts.GameCore.Sessions;
using Obisoft.HSharp.Models;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages.PlayerControl;
internal class SetAdminStatusPage : IHtmlPage
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();

    public string page => "setAdminStatus";

    public async Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var showActivePlayers = query["showActivePlayers"];
        var fromActivePlayers = showActivePlayers is not null;
        if (!long.TryParse(query["telegramId"], out var parsedTelegramId))
        {
            var error = HtmlHelper.CreateMessagePage("Incorrent Telegram ID", $"Incorrent 'telegramId'", localPath);
            response.AsTextUTF8(error.GenerateHTML());
            response.Close();
            return;
        }

        if (!sessionInfo.withoutLogin)
        {
            var db = BotController.dataBase.db;
            var profileData = db.Table<ProfileData>().Where(x => x.telegram_id == sessionInfo.telegramId).FirstOrDefault();
            if (profileData is null || profileData.adminStatus < AdminStatus.Root)
            {
                var error = HtmlHelper.CreateMessagePage("Forbidden", $"Required '{AdminStatus.Root}' status", GetBackUrl(query, localPath));
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
        }

        var adminStatus = query["adminStatus"];
        if (adminStatus is not null)
        {
            // set admin status
            if (!Enum.TryParse<AdminStatus>(adminStatus, ignoreCase: true, out var parsedAdminStatus))
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"Incorrent 'adminStatus'", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }

            await SetAdminStatus(response, sessionInfo, query, localPath, parsedTelegramId, parsedAdminStatus).FastAwait();
            return;
        }

        // show set admin status dialog
        var document = HtmlHelper.CreateDocument("Set Admin Status");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());

        var centerScreenBlock = new HTag("div", StylesHelper.CenterScreenBlock(700, 700));
        document["html"]["body"].Add(centerScreenBlock);

        var statusCollection = Enum.GetNames<AdminStatus>();

        var form = new HtmlFormBuilder(localPath)
            .AddHeader($"Set Admin Status (ID {parsedTelegramId})")
            .AddHiddenInput("page", page)
            .AddHiddenInput("telegramId", parsedTelegramId.ToString())
            .AddHiddenInputIfNotNull("showActivePlayers", showActivePlayers)
            .AddComboBox("adminStatus", statusCollection)
            .AddButton("Apply")
            .GetResult();

        centerScreenBlock.Add("div").Add(form);
        centerScreenBlock.Add("br");
        centerScreenBlock.Add("br");
        centerScreenBlock.Add(HtmlHelper.CreateLinkButton("<< Back", GetBackUrl(query, localPath)));

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task SetAdminStatus(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, long telegramId, AdminStatus adminStatus)
    {
        var session = sessionManager.GetSessionIfExists(telegramId);
        if (session is not null)
        {
            session.profile.data.adminStatus = adminStatus;
            var notification = Localization.Get(session, "notification_admin_set_admin_status", sessionInfo.GetAdminView(), adminStatus);
            session.profile.data.AddSpecialNotification(notification);
            await session.profile.SaveProfile().FastAwait();
            ShowSuccess(response, sessionInfo, query, localPath, session.profile.data, adminStatus);
        }
        else
        {
            var db = BotController.dataBase.db;
            var profileData = db.Table<ProfileData>().Where(x => x.telegram_id == telegramId).FirstOrDefault();
            if (profileData == null)
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"ProfileData with 'telegram_id' == {telegramId} not exists", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
            profileData.adminStatus = adminStatus;
            var notification = Localization.Get(profileData.language, "notification_admin_set_admin_status", sessionInfo.GetAdminView(), adminStatus);
            profileData.AddSpecialNotification(notification);
            db.Update(profileData);
            ShowSuccess(response, sessionInfo, query, localPath, profileData, adminStatus);
        }
    }

    private void ShowSuccess(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, ProfileData profileData, AdminStatus adminStatus)
    {
        var playerUser = new User
        {
            Id = profileData.telegram_id,
            FirstName = profileData.firstName,
            LastName = profileData.lastName,
            Username = profileData.username,
        };
        Program.logger.Info($"Administrator {sessionInfo.user} set player {playerUser} admin status to: {adminStatus}");

        var success = HtmlHelper.CreateMessagePage("Success", $"Admin status changed to '{adminStatus}' for {playerUser}", GetBackUrl(query, localPath));
        response.AsTextUTF8(success.GenerateHTML());
        response.Close();
    }

    private string GetBackUrl(NameValueCollection query, string localPath)
    {
        return localPath + $"?page=playerSearch&telegramId={query["telegramId"]}"
            + (query["showActivePlayers"] is not null ? "&showActivePlayers=" : string.Empty);
    }

}
