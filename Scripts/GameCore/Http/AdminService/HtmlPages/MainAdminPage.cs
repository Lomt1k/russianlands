using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.DailyDataManager;
using MarkOne.Scripts.GameCore.Services.DailyDataManagers;
using MarkOne.Scripts.GameCore.Sessions;
using Obisoft.HSharp.Models;
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
    private static readonly ServerDailyDataManager serverDailyDataManager = ServiceLocator.Get<ServerDailyDataManager>();
    private static readonly ProfileDailyDataManager profileDailyDataManager = ServiceLocator.Get<ProfileDailyDataManager>();

    public string page => "main";

    public async Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        // prepare data
        var allSessions = sessionManager.GetAllSessions();
        var dtNow = DateTime.UtcNow;
        var recentlyActive = allSessions.Where(x => (dtNow - x.lastActivityTime).TotalMinutes < 5).Count();
        var debugInfo = pm.debugInfo;
        var date = serverDailyDataManager.lastDate;
        var dailyActiveUsers = await profileDailyDataManager.GetDailyActiveUsers().FastAwait();

        // prepare document
        var document = HtmlHelper.CreateDocument("Main Admin Page");

        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());
        var centerScreenBlock = new HTag("div", new HProp("align", "center"), StylesHelper.CenterScreenBlock(700, 500));
        document["html"]["body"].AddChild(centerScreenBlock);

        // status table
        centerScreenBlock.AddChild("p", $"Server Stats ({date.ToShortDateString()})");
        var table = new HTag("table", new HProp("frame", "hsides"));
        table.AddChild(CreateTableRow("DAU", dailyActiveUsers.ToString()));
        table.AddChild(CreateTableRow("Sessions", allSessions.Count.ToString()));
        table.AddChild(CreateTableRow("Now playing", recentlyActive.ToString()));
        table.AddChild(CreateTableRow("&nbsp;", "&nbsp;"));
        table.AddChild(CreateTableRow("Status", pm.currentState.ToString()));
        table.AddChild(CreateTableRow("CPU", debugInfo.cpuInfo.Replace("CPU: ", string.Empty)));
        table.AddChild(CreateTableRow("RAM", debugInfo.memoryInfo.Replace("RAM: ", string.Empty)));
        table.AddChild(CreateTableRow("Total RAM", debugInfo.totalMemoryInfo.Replace("Total RAM: ", string.Empty)));        
        centerScreenBlock.AddChild(table);

        // top buttons
        var topButtons = new HTag("div", new HProp("style", "margin: 15px 0;"));
        topButtons.AddChild(HtmlHelper.CreateLinkButton("Show Log", localPath + "?page=showLog"));
        topButtons.AddChild(HtmlHelper.CreateLinkButton("Show Errors", localPath + "?page=showLog&mode=errors"));
        centerScreenBlock.AddChild(topButtons);

        // other buttons
        var otherButtons = new HTag("div", new HProp("style", "margin: 200px 0;"));
        otherButtons.AddChild(HtmlHelper.CreateLinkButton("Player Search", localPath + "?page=playerSearch", color: "#808080"));
        centerScreenBlock.AddChild(otherButtons);

        // send document
        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
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
