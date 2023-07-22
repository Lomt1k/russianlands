using Obisoft.HSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages;
internal class ShowLogPage : IHtmlPage
{
    public string page => "showLog";

    public async Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var mode = query["mode"];
        if (mode != null)
        {
            if (mode.Equals("list"))
            {
                ShowFilesList(response, localPath);
                return;
            }
            if (mode.Equals("search"))
            {
                await ShowLogsWithSearch(response, query, localPath).FastAwait();
                return;
            }
        }

        var fileName = query["fileName"];
        if (string.IsNullOrEmpty(fileName))
        {
            var error = HtmlHelper.CreateMessagePage("Show Log List", $"File with name '" + (fileName is null ? string.Empty : fileName) + "' not found", localPath + $"?page={page}");
            response.AsTextUTF8(error.GenerateHTML());
            response.Close();
            return;
        }

        var filePath = Path.Combine("Logs", fileName);
        var lines = File.Exists(filePath)
            ? await ReadTextFromFile(filePath).FastAwait()
            : new List<string>() { $"Log not exists :(\nPath: {filePath}" };

        var document = HtmlHelper.CreateDocument($"Show Log [{fileName}]");
        var contentBlock = new HTag("div");
        document["html"]["body"].Add(contentBlock);

        var logTable = new HTag("table", new HProp("frame", "hsides"));
        var sb = new StringBuilder();
        foreach (var  line in lines)
        {
            sb.Append(line + "<br>");
        }
        var tableLine = new HTag("tr")
        {
            { "td", sb.ToString() }
        };
        logTable.Add(tableLine);

        contentBlock.Add(logTable);

        var bottomPanel = new HTag("div", new HProp("align", "center"), new HProp("style", "margin: 40px 0;"))
        {
            HtmlHelper.CreateLinkButton("<< Back", localPath)
        };
        contentBlock.Add(bottomPanel);

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private void ShowFilesList(HttpListenerResponse response, string localPath)
    {
        var document = HtmlHelper.CreateDocument("Show Logs List");
        foreach (var filePath in Directory.EnumerateFiles("Logs"))
        {
            var fileName = Path.GetFileName(filePath);
            var div = new HTag("h2")
            {
                { "a", fileName, new HProp("href", localPath + $"?page={page}&fileName={fileName}")}
            };
            document["html"]["body"].Add(div);
        }

        var bottomPanel = new HTag("div", new HProp("align", "center"), new HProp("style", "margin: 40px 0;"))
        {
            HtmlHelper.CreateLinkButton("<< Back", localPath)
        };
        document["html"]["body"].Add(bottomPanel);

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task ShowLogsWithSearch(HttpListenerResponse response, NameValueCollection query, string localPath)
    {
        var searchId = query["searchId"];
        bool fromActivePlayers = query["showActivePlayers"] is not null;
        if (searchId is null)
        {
            // TODO
            return;
        }

        // search logs
        var searchPattern = $"(ID {searchId})";
        var fileNames = Directory.EnumerateFiles("Logs").Where(x => x.Contains("appLog")).ToArray();
        var fileInfos = new FileInfo[fileNames.Length];
        for (int i = 0; i < fileNames.Length; i++)
        {
            fileInfos[i] = new FileInfo(fileNames[i]);
        }
        var sortedFileInfos = fileInfos.OrderBy(x => x.CreationTime);
        
        var resultLogs = new List<string>();
        foreach (var file in sortedFileInfos)
        {
            var strings = await ReadTextFromFile(file.FullName).FastAwait();
            foreach (var str in strings)
            {
                if (str.Contains(searchPattern))
                {
                    resultLogs.Add(str);
                }
            }
        }

        // show results
        var document = HtmlHelper.CreateDocument($"Show Log :: {searchPattern}");
        var contentBlock = new HTag("div");
        document["html"]["body"].Add(contentBlock);

        var logTable = new HTag("table", new HProp("frame", "hsides"));
        var sb = new StringBuilder();
        foreach (var line in resultLogs)
        {
            sb.Append(line + "<br>");
        }
        var tableLine = new HTag("tr")
        {
            { "td", sb.ToString() }
        };
        logTable.Add(tableLine);

        contentBlock.Add(logTable);

        var bottomPanel = new HTag("div", new HProp("align", "center"), new HProp("style", "margin: 40px 0;"))
        {
            HtmlHelper.CreateLinkButton("<< Back", localPath + $"?page=playerSearch&telegramId={searchId}" + (fromActivePlayers ? "&showActivePlayers=" : string.Empty) )
        };
        contentBlock.Add(bottomPanel);

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
