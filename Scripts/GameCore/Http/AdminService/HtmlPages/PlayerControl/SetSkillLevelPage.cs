using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Skills;
using Obisoft.HSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages.PlayerControl;
internal class SetSkillLevelPage : IHtmlPage
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();

    public string page => "setSkillLevel";

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

        var skillType = query["skillType"];
        var level = query["level"];
        if (skillType is not null && level is not null)
        {
            // set skill level
            if (!Enum.TryParse<ItemType>(skillType, ignoreCase: true, out var parsedSkillType) || !byte.TryParse(level, out var parsedLevel))
            {
                var error = HtmlHelper.CreateMessagePage("Bad request", $"Incorrent 'skillType' or 'level'", localPath);
                response.AsTextUTF8(error.GenerateHTML());
                response.Close();
                return;
            }

            await SetSkillLevel(response, sessionInfo, query, localPath, parsedTelegramId, parsedSkillType, parsedLevel).FastAwait();
            return;
        }

        // show set skill level dialog
        var document = HtmlHelper.CreateDocument("Set Skill Level");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());

        var centerScreenBlock = new HTag("div", StylesHelper.CenterScreenBlock(700, 700));
        document["html"]["body"].Add(centerScreenBlock);

        var skillTypes = new List<string>();
        foreach (var id in SkillsDictionary.GetAllSkillTypes())
        {
            skillTypes.Add(id.ToString());
        }

        var form = new HtmlFormBuilder(localPath)
            .AddHeader($"Set Skill Level (ID {parsedTelegramId})")
            .AddHiddenInput("page", page)
            .AddHiddenInput("telegramId", parsedTelegramId.ToString())
            .AddHiddenInputIfNotNull("showActivePlayers", showActivePlayers)
            .AddComboBox("skillType", skillTypes)
            .AddInput("level", "level")
            .AddButton("Set Level")
            .GetResult();

        centerScreenBlock.Add("div").Add(form);
        centerScreenBlock.Add("br");
        centerScreenBlock.Add("br");
        centerScreenBlock.Add(HtmlHelper.CreateLinkButton("<< Back", GetBackUrl(query, localPath)));

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task SetSkillLevel(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, long telegramId, ItemType skillType, byte level)
    {
        var session = sessionManager.GetSessionIfExists(telegramId);
        if (session is not null)
        {
            session.player.skills.SetValue(skillType, level);
            var skillView = skillType.GetEmoji() + skillType.GetCategoryLocalization(session) + $": {level}";
            var notification = Localization.Get(session, "notification_admin_set_skill_level", sessionInfo.GetAdminView(), skillView);
            session.profile.data.AddSpecialNotification(notification);
            await session.profile.SaveProfile().FastAwait();
            ShowSuccess(response, sessionInfo, query, localPath, session.profile.data, skillView);
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
            SkillsDictionary.Get(skillType).SetValue(profileData, level);
            var skillView = skillType.GetEmoji() + skillType.GetCategoryLocalization(profileData.language) + $": {level}";
            var notification = Localization.Get(profileData.language, "notification_admin_set_skill_level", sessionInfo.GetAdminView(), skillView);
            profileData.AddSpecialNotification(notification);
            await db.UpdateAsync(profileData).FastAwait();
            ShowSuccess(response, sessionInfo, query, localPath, profileData, skillView);
        }
    }

    private void ShowSuccess(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath, ProfileData profileData, string skillView)
    {
        var playerUser = new SimpleUser
        {
            id = profileData.telegram_id,
            firstName = profileData.firstName,
            lastName = profileData.lastName,
            username = profileData.username,
        };
        Program.logger.Info($"Administrator {sessionInfo.user} set player {playerUser} skill level: {skillView}");

        var success = HtmlHelper.CreateMessagePage("Success", $"Skill level changed\n{skillView}\nfor {playerUser}", GetBackUrl(query, localPath));
        response.AsTextUTF8(success.GenerateHTML());
        response.Close();
    }

    private string GetBackUrl(NameValueCollection query, string localPath)
    {
        return localPath + $"?page=playerSearch&telegramId={query["telegramId"]}"
            + (query["showActivePlayers"] is not null ? "&showActivePlayers=" : string.Empty);
    }

}
