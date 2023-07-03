using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using Obisoft.HSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Buildings;
using FastTelegramBot.DataTypes;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages.PlayerControl;
internal class SetBuildingLevelPage : IHtmlPage
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();

    public string page => "setBuildingLevel";

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
            var profileData = await db.Table<ProfileData>().Where(x => x.telegram_id == sessionInfo.telegramId).FirstOrDefaultAsync().FastAwait();
            if (profileData is null || profileData.adminStatus < AdminStatus.Admin)
            {
                var error = HtmlHelper.CreateMessagePage("Forbidden", $"You dont have admin rights", GetBackUrl(query, localPath));
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
        }

        var buildingId = query["buildingId"];
        var level = query["level"];
        if (buildingId is not null && level is not null)
        {
            // set building level
            if (!Enum.TryParse<BuildingId>(buildingId, ignoreCase: true, out var parsedBuildingId) || !byte.TryParse(level, out var parsedLevel))
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"Incorrent 'buildingId' or 'level'", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }

            await SetBuildingLevel(response, sessionInfo, query, localPath, parsedTelegramId, parsedBuildingId, parsedLevel).FastAwait();
            return;
        }

        // show set building level dialog
        var document = HtmlHelper.CreateDocument("Set Building Level");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());

        var centerScreenBlock = new HTag("div", StylesHelper.CenterScreenBlock(700, 700));
        document["html"]["body"].Add(centerScreenBlock);

        var buildingIds = new List<string>();
        foreach (var id in Enum.GetValues<BuildingId>())
        {
            buildingIds.Add(id.ToString());
        }

        var form = new HtmlFormBuilder(localPath)
            .AddHeader($"Set Building Level (ID {parsedTelegramId})")
            .AddHiddenInput("page", page)
            .AddHiddenInput("telegramId", parsedTelegramId.ToString())
            .AddHiddenInputIfNotNull("showActivePlayers", showActivePlayers)
            .AddComboBox("buildingId", buildingIds)
            .AddInput("level", "level")
            .AddButton("Set level")
            .GetResult();

        centerScreenBlock.Add("div").Add(form);
        centerScreenBlock.Add("br");
        centerScreenBlock.Add("br");
        centerScreenBlock.Add(HtmlHelper.CreateLinkButton("<< Back", GetBackUrl(query, localPath)));

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task SetBuildingLevel(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, long telegramId, BuildingId buildingId, byte level)
    {
        var session = sessionManager.GetSessionIfExists(telegramId);
        if (session is not null)
        {
            buildingId.GetBuilding().Cheat_SetCurrentLevel(session.profile.buildingsData, level);
            var notification = Localization.Get(session, "notification_admin_set_building_level", sessionInfo.GetAdminView(), buildingId.GetBuilding().GetLocalizedName(session, session.profile.buildingsData));
            session.profile.data.AddSpecialNotification(notification);
            await session.profile.SaveProfile().FastAwait();
            ShowSuccessfullAddResource(response, sessionInfo, query, localPath, session.profile.data, session.profile.buildingsData, buildingId);
        }
        else
        {
            var db = BotController.dataBase.db;
            var profileData = await db.Table<ProfileData>().Where(x => x.telegram_id == telegramId).FirstOrDefaultAsync().FastAwait();
            if (profileData == null)
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"ProfileData with 'telegram_id' == {telegramId} not exists", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
            var buildingsData = await db.Table<ProfileBuildingsData>().Where(x => x.dbid == profileData.dbid).FirstOrDefaultAsync().FastAwait();
            if (buildingsData == null)
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"ProfileBuildingsData for 'telegram_id' == {telegramId} not exists", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
            buildingId.GetBuilding().Cheat_SetCurrentLevel(buildingsData, level);
            var notification = Localization.Get(profileData.language, "notification_admin_add_resource", sessionInfo.GetAdminView(), buildingId.GetBuilding().GetLocalizedName(profileData.language, buildingsData));
            profileData.AddSpecialNotification(notification);
            await db.UpdateAsync(profileData).FastAwait();
            await db.UpdateAsync(buildingsData).FastAwait();
            ShowSuccessfullAddResource(response, sessionInfo, query, localPath, profileData, buildingsData, buildingId);
        }
    }

    private void ShowSuccessfullAddResource(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, ProfileData profileData, ProfileBuildingsData buildingsData, BuildingId buildingId)
    {
        var playerUser = new User
        {
            Id = profileData.telegram_id,
            FirstName = profileData.firstName,
            LastName = profileData.lastName,
            Username = profileData.username,
        };
        Program.logger.Info($"Administrator {sessionInfo.user} changed the level of the building for player {playerUser}: {buildingId.GetBuilding().GetLocalizedName(sessionInfo.languageCode, buildingsData)}");

        var success = HtmlHelper.CreateMessagePage("Success", $"Building level changed\n{buildingId.GetBuilding().GetLocalizedName(sessionInfo.languageCode, buildingsData)}\nfor {playerUser}", GetBackUrl(query, localPath));
        response.AsTextUTF8(success.GenerateHTML());
        response.Close();
    }

    private string GetBackUrl(NameValueCollection query, string localPath)
    {
        return localPath + $"?page=playerSearch&telegramId={query["telegramId"]}"
            + (query["showActivePlayers"] is not null ? "&showActivePlayers=" : string.Empty);
    }

}
