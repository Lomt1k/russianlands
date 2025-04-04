﻿using FastTelegramBot.DataTypes;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using MarkOne.Scripts.GameCore.Sessions;
using Obisoft.HSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages.PlayerControl;
internal class AddResourcePage : IHtmlPage
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();

    public string page => "addResource";

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
            if (profileData is null || profileData.adminStatus < AdminStatus.Admin)
            {
                var error = HtmlHelper.CreateMessagePage("Forbidden", $"You dont have admin rights", GetBackUrl(query, localPath));
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
        }

        var resourceId = query["resourceId"];
        var amount = query["amount"];
        if (resourceId is not null && amount is not null)
        {
            // add resource
            if (!Enum.TryParse<ResourceId>(resourceId, ignoreCase: true, out var parsedResourceId) || !int.TryParse(amount, out var parsedAmount))
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"Incorrent 'amount' or 'resourceId'", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
            
            await AddResource(response, sessionInfo, query, localPath, parsedTelegramId, new ResourceData(parsedResourceId, parsedAmount)).FastAwait();
            return;
        }

        // show add resource dialog
        var document = HtmlHelper.CreateDocument("Add Resource");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());

        var centerScreenBlock = new HTag("div", StylesHelper.CenterScreenBlock(700, 700));
        document["html"]["body"].Add(centerScreenBlock);

        var resourceIds = new List<string>();
        foreach (var id in Enum.GetValues<ResourceId>())
        {
            resourceIds.Add(id.ToString());
        }

        var form = new HtmlFormBuilder(localPath)
            .AddHeader($"Add Resource (ID {parsedTelegramId})")
            .AddHiddenInput("page", page)
            .AddHiddenInput("telegramId", parsedTelegramId.ToString())
            .AddHiddenInputIfNotNull("showActivePlayers", showActivePlayers)
            .AddComboBox("resourceId", resourceIds)
            .AddInput("amount", "amount")
            .AddButton("Add")
            .GetResult();

        centerScreenBlock.Add("div").Add(form);
        centerScreenBlock.Add("br");
        centerScreenBlock.Add("br");
        centerScreenBlock.Add(HtmlHelper.CreateLinkButton("<< Back", GetBackUrl(query, localPath)));

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task AddResource(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, long telegramId, ResourceData resourceData)
    {
        var session = sessionManager.GetSessionIfExists(telegramId);
        if (session is not null)
        {
            session.player.resources.ForceAdd(resourceData);
            var notification = Localization.Get(session, "notification_admin_add_resource", sessionInfo.GetAdminView(), resourceData.GetLocalizedView(session));
            session.profile.data.AddSpecialNotification(notification);
            await session.profile.SaveProfile().FastAwait();
            ShowSuccessfullAddResource(response, sessionInfo, query, localPath, session.profile.data, resourceData);
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
            ResourcesDictionary.Get(resourceData.resourceId).AddValue(profileData, resourceData.amount);
            var notification = Localization.Get(profileData.language, "notification_admin_add_resource", sessionInfo.GetAdminView(), resourceData.GetLocalizedView(profileData.language));
            profileData.AddSpecialNotification(notification);
            db.Update(profileData);
            ShowSuccessfullAddResource(response, sessionInfo, query, localPath, profileData, resourceData);
        }
    }

    private void ShowSuccessfullAddResource(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, ProfileData profileData, ResourceData resourceData)
    {
        var playerUser = new User
        {
            Id = profileData.telegram_id,
            FirstName = profileData.firstName,
            LastName = profileData.lastName,
            Username = profileData.username,
        };
        Program.logger.Info($"Administrator {sessionInfo.user} gave the player {playerUser} {resourceData.GetLocalizedView(sessionInfo.languageCode)}");

        var success = HtmlHelper.CreateMessagePage("Success", $"Successfully adds\n{resourceData.GetLocalizedView(sessionInfo.languageCode)}\nfor {playerUser}", GetBackUrl(query, localPath));
        response.AsTextUTF8(success.GenerateHTML());
        response.Close();
    }

    private string GetBackUrl(NameValueCollection query, string localPath)
    {
        return localPath + $"?page=playerSearch&telegramId={query["telegramId"]}"
            + (query["showActivePlayers"] is not null ? "&showActivePlayers=" : string.Empty );
    }

}
