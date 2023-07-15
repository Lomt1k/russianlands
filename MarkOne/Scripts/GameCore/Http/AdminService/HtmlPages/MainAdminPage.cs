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
        var dailyRevenue = await serverDailyDataManager.GetIntegerValue("revenue").FastAwait();
        var dailyActiveUsers = await profileDailyDataManager.GetDailyActiveUsers().FastAwait();

        // prepare document
        var document = HtmlHelper.CreateDocument("Main Admin Page");

        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());
        var centerScreenBlock = new HTag("div", new HProp("align", "center"), StylesHelper.CenterScreenBlock(700, 500));
        document["html"]["body"].Add(centerScreenBlock);

        // status table
        centerScreenBlock.Add("p", $"Server Stats ({date.ToShortDateString()})");
        var table = new HTag("table", new HProp("frame", "hsides"))
        {
            CreateTableRow("DAU", dailyActiveUsers.ToString()),
            CreateTableRow("Sessions", allSessions.Count.ToString()),
            CreateTableRow("Now playing", recentlyActive.ToString()),
            CreateTableRow("Revenue", $"{dailyRevenue} RUB"),
            CreateTableRow("&nbsp;", "&nbsp;"),
            CreateTableRow("Status", pm.currentState.ToString()),
            CreateTableRow("CPU", debugInfo.cpuInfo.Replace("CPU: ", string.Empty)),
            CreateTableRow("RAM", debugInfo.memoryInfo.Replace("RAM: ", string.Empty)),
            CreateTableRow("Total RAM", debugInfo.totalMemoryInfo.Replace("Total RAM: ", string.Empty))
        };
        centerScreenBlock.Add(table);

        // top buttons
        var topButtons = new HTag("div", new HProp("style", "margin: 15px 0;"))
        {
            HtmlHelper.CreateLinkButton("Last Log", localPath + "?page=showLog&fileName=appLog.log"),
            HtmlHelper.CreateLinkButton("Last Errors", localPath + "?page=showLog&fileName=errors.log"),
            HtmlHelper.CreateLinkButton("Logs List", localPath + "?page=showLog&mode=list"),
        };
        centerScreenBlock.Add(topButtons);

        // other buttons
        var otherButtons = new HTag("div", new HProp("style", "margin: 200px 0;"))
        {
            HtmlHelper.CreateLinkButton("Active Players", localPath + "?page=playerSearch&showActivePlayers=", color: "#808080"),
            HtmlHelper.CreateLinkButton("Player Search", localPath + "?page=playerSearch", color: "#808080"),
            HtmlHelper.CreateLinkButton("News Editor", localPath + "?page=newsEditor", color: "#808080"),
        };
        centerScreenBlock.Add(otherButtons);

        // send document
        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private HTag CreateTableRow(string header, params string[] args)
    {
        var row = new HTag("tr")
        {
            { "th", header }
        };
        foreach (var arg in args)
        {
            var record = new HTag("td", arg);
            record.AddProperties(new HProp("align", "right"));
            row.Add(record);
        }
        return row;
    }
}
