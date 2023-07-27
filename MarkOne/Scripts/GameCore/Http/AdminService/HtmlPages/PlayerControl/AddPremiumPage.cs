using FastTelegramBot.DataTypes;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;
using Obisoft.HSharp.Models;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages.PlayerControl;
internal class AddPremiumPage : IHtmlPage
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();

    public string page => "addPremium";

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

        var amount = query["amount"];
        if (amount is not null)
        {
            // add resource
            if (!int.TryParse(amount, out var parsedAmount))
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"Incorrent 'amount'", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }

            await AddPremium(response, sessionInfo, query, localPath, parsedTelegramId, parsedAmount).FastAwait();
            return;
        }

        // show add premium dialog
        var document = HtmlHelper.CreateDocument("Add Premium");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());

        var centerScreenBlock = new HTag("div", StylesHelper.CenterScreenBlock(700, 700));
        document["html"]["body"].Add(centerScreenBlock);

        var form = new HtmlFormBuilder(localPath)
            .AddHeader($"Add Premium (ID {parsedTelegramId})")
            .AddHiddenInput("page", page)
            .AddHiddenInput("telegramId", parsedTelegramId.ToString())
            .AddHiddenInputIfNotNull("showActivePlayers", showActivePlayers)
            .AddInput("amount", "days")
            .AddButton("Add")
            .GetResult();

        centerScreenBlock.Add("div").Add(form);
        centerScreenBlock.Add("br");
        centerScreenBlock.Add("br");
        centerScreenBlock.Add(HtmlHelper.CreateLinkButton("<< Back", GetBackUrl(query, localPath)));

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task AddPremium(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, long telegramId, int daysAmount)
    {
        var session = sessionManager.GetSessionIfExists(telegramId);
        if (session is not null)
        {
            var premiumTimeView = TimeSpan.FromDays(daysAmount).GetShortView(session);
            AddPremiumValue(session.profile.data, daysAmount);
            var notification = Localization.Get(session, "notification_admin_add_premium", sessionInfo.GetAdminView(), Emojis.StatPremium, premiumTimeView);
            session.profile.data.AddSpecialNotification(notification);
            await session.profile.SaveProfile().FastAwait();
            ShowSuccessfullAddPremium(response, sessionInfo, query, localPath, session.profile.data, daysAmount);
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
            var premiumTimeView = TimeSpan.FromDays(daysAmount).GetShortView(profileData.language);
            AddPremiumValue(profileData, daysAmount);
            var notification = Localization.Get(profileData.language, "notification_admin_add_premium", sessionInfo.GetAdminView(), Emojis.StatPremium, premiumTimeView);
            profileData.AddSpecialNotification(notification);
            db.Update(profileData);
            ShowSuccessfullAddPremium(response, sessionInfo, query, localPath, profileData, daysAmount);
        }
    }

    private void AddPremiumValue(ProfileData profileData, int daysAmount)
    {
        var endPremiumTime = profileData.IsPremiumActive() ? profileData.endPremiumTime : DateTime.UtcNow;
        profileData.endPremiumTime = endPremiumTime.AddDays(daysAmount);
    }

    private void ShowSuccessfullAddPremium(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, ProfileData profileData, int daysAmount)
    {
        var playerUser = new User
        {
            Id = profileData.telegram_id,
            FirstName = profileData.firstName,
            LastName = profileData.lastName,
            Username = profileData.username,
        };
        var premiumTimeView = TimeSpan.FromDays(daysAmount).GetShortView(profileData.language);
        Program.logger.Info($"Administrator {sessionInfo.user} gave the player {playerUser} Premium ({daysAmount} days)");

        var success = HtmlHelper.CreateMessagePage("Success", $"Successfully adds\n{Emojis.StatPremium} Premium: {premiumTimeView}\nfor {playerUser}", GetBackUrl(query, localPath));
        response.AsTextUTF8(success.GenerateHTML());
        response.Close();
    }

    private string GetBackUrl(NameValueCollection query, string localPath)
    {
        return localPath + $"?page=playerSearch&telegramId={query["telegramId"]}"
            + (query["showActivePlayers"] is not null ? "&showActivePlayers=" : string.Empty);
    }
}
