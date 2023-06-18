using MarkOne.Scripts.GameCore.Resources;
using Obisoft.HSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages.PlayerControl;
internal class AddResourcePage : IHtmlPage
{
    public string page => "addResource";

    public Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var showActivePlayers = query["showActivePlayers"];
        var fromActivePlayers = showActivePlayers is not null;
        var telegramId = query["telegramId"];
        if (telegramId is null)
        {
            var error = HtmlHelper.CreateErrorPage("Incorrent Telegram ID", $"Incorrent 'telegramId'", localPath + $"?page={page}");
            response.AsTextUTF8(error.GenerateHTML());
            response.Close();
            return Task.CompletedTask;
        }

        var resourceId = query["resourceId"];
        var amount = query["amount"];
        if (resourceId is not null && amount is not null)
        {
            // add resource id
            // TODO
        }

        // show add resource id dialog
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
            .AddHeader($"Add Resource (ID {telegramId})")
            .AddHiddenInput("page", page)
            .AddHiddenInput("telegramId", telegramId)
            .AddHiddenInputIfNotNull("showActivePlayers", showActivePlayers)
            .AddComboBox("resourceId", resourceIds)
            .AddInput("amount", "amount")
            .AddButton("Add")
            .GetResult();

        centerScreenBlock.Add("div").Add(form);
        centerScreenBlock.Add(
            HtmlHelper.CreateLinkButton("<< Back", localPath + $"?page=playerSearch" + $"&telegramId={telegramId}" + (fromActivePlayers ? "&showActivePlayers=" : string.Empty))
            );

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
        return Task.CompletedTask;
    }
}
