using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;
using Obisoft.HSharp.Models;
using SimpleHttp;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages;
internal class MainAdminPage : IHtmlPage
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();
    private static readonly PerformanceManager pm = ServiceLocator.Get<PerformanceManager>();

    public string page => "main";

    public Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        // prepare data
        var allSessions = sessionManager.GetAllSessions();
        var dtNow = DateTime.UtcNow;
        var recentlyActive = allSessions.Where(x => (dtNow - x.lastActivityTime).TotalMinutes < 5).Count();
        var debugInfo = pm.debugInfo;

        // prepare document
        var Document = HtmlHelper.CreateDocument("Main Admin Page");

        Document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());
        var centerScreenBlock = new HTag("div", StylesHelper.CenterScreenBlock(700, 250));
        Document["html"]["body"].AddChild(centerScreenBlock);

        // status table
        var table = new HTag("table", new HProp("frame", "hsides"), new HProp("align", "center"));
        table.AddChild(CreateTableRow("Sessions", allSessions.Count.ToString()));
        table.AddChild(CreateTableRow("Now playing", recentlyActive.ToString()));
        table.AddChild(CreateTableRow("&nbsp;", "&nbsp;"));
        table.AddChild(CreateTableRow("Status", pm.currentState.ToString()));
        table.AddChild(CreateTableRow("CPU", debugInfo.cpuInfo.Replace("CPU: ", string.Empty)));
        table.AddChild(CreateTableRow("RAM", debugInfo.memoryInfo.Replace("RAM: ", string.Empty)));
        table.AddChild(CreateTableRow("Total RAM", debugInfo.totalMemoryInfo.Replace("Total RAM: ", string.Empty)));
        
        centerScreenBlock.AddChild(table);

        // TODO next div
        var nextDiv = new HTag("div", new HProp("align", "center"));        
        nextDiv.AddChild("p", "Hello friend");
        centerScreenBlock.AddChild(nextDiv);

        // send document
        response.AsText(Document.GenerateHTML());
        response.Close();
        return Task.CompletedTask;
    }

    private HTag CreateTableRow(string header, params string[] args)
    {
        var row = new HTag("tr");
        row.AddChild("th", header);
        foreach (var arg in args)
        {
            var record = new HTag("td", arg);
            record.AddProperties(new HProp("align", "right"));
            row.AddChild(record);
        }
        return row;
    }
}
