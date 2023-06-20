using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Resources;
using Obisoft.HSharp.Models;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Services;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages.PlayerControl;
internal class AddItemPage : IHtmlPage
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();

    public string page => "addItem";

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

        var itemCode = query["itemCode"];
        if (itemCode is not null)
        {
            // add item
            try
            {
                var item = new InventoryItem(itemCode);
                if (item is null || string.IsNullOrEmpty(item.GetFullName(sessionInfo.languageCode)))
                {
                    var error = HtmlHelper.CreateMessagePage("Bad request", $"Incorrent 'itemCode'", localPath);
                    response.AsTextUTF8(error.GenerateHTML());
                    response.Close();
                    return;
                }
                await AddItem(response, sessionInfo, query, localPath, parsedTelegramId, item).FastAwait();
                return;
            }
            catch (Exception ex)
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"Catched exception on try create item with code '{itemCode}'", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }
        }

        // show add item dialog
        var document = HtmlHelper.CreateDocument("Add Item");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());

        var centerScreenBlock = new HTag("div", StylesHelper.CenterScreenBlock(700, 700));
        document["html"]["body"].Add(centerScreenBlock);

        var resourceIds = new List<string>();
        foreach (var id in Enum.GetValues<ResourceId>())
        {
            resourceIds.Add(id.ToString());
        }

        var form = new HtmlFormBuilder(localPath)
            .AddHeader($"Add Item (ID {parsedTelegramId})")
            .AddHiddenInput("page", page)
            .AddHiddenInput("telegramId", parsedTelegramId.ToString())
            .AddHiddenInputIfNotNull("showActivePlayers", showActivePlayers)
            .AddInput("itemCode", "item code / id")
            .AddButton("Add")
            .GetResult();

        centerScreenBlock.Add("div").Add(form);
        centerScreenBlock.Add("br");
        centerScreenBlock.Add("br");
        centerScreenBlock.Add(HtmlHelper.CreateLinkButton("<< Back", GetBackUrl(query, localPath)));

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task AddItem(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, long telegramId, InventoryItem item)
    {
        var session = sessionManager.GetSessionIfExists(telegramId);
        if (session is not null)
        {
            session.player.inventory.ForceAddItem(item);
            var notification = Localization.Get(session, "notification_admin_add_resource", sessionInfo.GetAdminView(), item.GetFullName(session));
            session.profile.data.AddSpecialNotification(notification);
            await session.profile.SaveProfile().FastAwait();
            ShowSuccessfullAddItem(response, sessionInfo, query, localPath, session.profile.data, item);
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
            var rawDynamicData = await db.Table<RawProfileDynamicData>().Where(x => x.dbid == profileData.dbid).FirstOrDefaultAsync().FastAwait();
            if (rawDynamicData == null)
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"RawProfileDynamicData for 'telegram_id' == {telegramId} not exists", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }

            var profileDynamicData = ProfileDynamicData.Deserialize(rawDynamicData);
            profileDynamicData.inventory.ForceAddItem(item);
            rawDynamicData.Fill(profileDynamicData);            

            var notification = Localization.Get(profileData.language, "notification_admin_add_resource", sessionInfo.GetAdminView(), item.GetFullName(profileData.language));
            profileData.AddSpecialNotification(notification);
            await db.UpdateAsync(rawDynamicData).FastAwait();
            ShowSuccessfullAddItem(response, sessionInfo, query, localPath, profileData, item);
        }
    }

    private void ShowSuccessfullAddItem(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, ProfileData profileData, InventoryItem item)
    {
        var playerUser = new SimpleUser
        {
            id = profileData.telegram_id,
            firstName = profileData.firstName,
            lastName = profileData.lastName,
            username = profileData.username,
        };
        Program.logger.Info($"Administrator {sessionInfo.user} gave the player {playerUser} {item.GetFullName(sessionInfo.languageCode)}");

        var success = HtmlHelper.CreateMessagePage("Success", $"Successfully adds\n{item.GetFullName(sessionInfo.languageCode)}\nfor {playerUser}", GetBackUrl(query, localPath));
        response.AsTextUTF8(success.GenerateHTML());
        response.Close();
    }

    private string GetBackUrl(NameValueCollection query, string localPath)
    {
        return localPath + $"?page=playerSearch&telegramId={query["telegramId"]}"
            + (query["showActivePlayers"] is not null ? "&showActivePlayers=" : string.Empty);
    }

}
