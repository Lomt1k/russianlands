using Obisoft.HSharp.Models;
using SimpleHttp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages;
internal class ShowLogPage : IHtmlPage
{
    public string page => "showLog";

    public async Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var mode = query["mode"];
        var fileName = mode switch
        {
            "errors" => "errors.log",
            _ => "appLog.log"
        };

        var filePath = Path.Combine("Logs", fileName);
        var lines = File.Exists(filePath)
            ? await ReadTextFromFile(filePath).FastAwait()
            : new List<string>() { $"Log not exists :(\nPath: {filePath}" };

        var document = HtmlHelper.CreateDocument($"Show Log [{fileName}]");
        var contentBlock = new HTag("div");
        document["html"]["body"].AddChild(contentBlock);

        var logTable = new HTag("table", new HProp("frame", "hsides"));
        foreach (var line in lines)
        {
            var tableLine = new HTag("tr");
            tableLine.AddChild("td", line);
            logTable.AddChild(tableLine);
        }
        contentBlock.AddChild(logTable);

        var bottomPanel = new HTag("div", new HProp("align", "center"), new HProp("style", "margin: 40px 0;"));
        bottomPanel.AddChild(HtmlHelper.CreateLinkButton("<< Back", localPath));
        contentBlock.AddChild(bottomPanel);

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task<List<string>> ReadTextFromFile(string filepath)
    {
        var lines = new List<string>();
        try
        {
            using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync().FastAwait();
                        if (line == null)
                        {
                            return lines;
                        }
                        lines.Add(line);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return new List<string> { ex.ToString() };
        }
        return lines;
    }
}
