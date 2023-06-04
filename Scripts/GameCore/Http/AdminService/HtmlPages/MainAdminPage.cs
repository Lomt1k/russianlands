using Obisoft.HSharp.Models;
using SimpleHttp;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages;
internal class MainAdminPage : IHtmlPage
{
    public string page => "main";

    public Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var Document = HtmlHelper.CreateDocument("Main Admin Page");

        Document["html"]["body"].AddChild("div");
        Document["html"]["body"]["div"].AddChild("a", new HProp("href", localPath));
        Document["html"]["body"]["div"]["a"].AddChild("p", "Hello friend");
        var Result = Document.GenerateHTML();

        response.AsText(Result);
        response.Close();
        return Task.CompletedTask;
    }
}
