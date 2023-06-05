using Obisoft.HSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages;
internal class PlayerSearchPage : IHtmlPage
{
    public string page => "playerSearch";

    public Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        // prepare document
        var document = HtmlHelper.CreateDocument("Player States");

        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());
        var centerScreenBlock = new HTag("div", new HProp("align", "center"), StylesHelper.CenterScreenBlock(700, 500));
        document["html"]["body"].AddChild(centerScreenBlock);

        // status table
        var searchByIdForm = HtmlHelper.CreateForm(localPath, page, "Search", new InputFieldInfo("telegramId", "Search by Telegram ID"));
        centerScreenBlock.AddChild(searchByIdForm);

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
        return Task.CompletedTask;
    }
}
