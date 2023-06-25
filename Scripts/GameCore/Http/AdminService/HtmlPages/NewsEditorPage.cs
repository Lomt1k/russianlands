using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using MarkOne.Scripts.GameCore.Services.News;
using Obisoft.HSharp.Models;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Http.AdminService.HtmlPages;
public class NewsEditorPage : IHtmlPage
{
    private readonly NewsService newsService = ServiceLocator.Get<NewsService>();

    public string page => "newsEditor";

    public async Task ShowPage(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var mode = query["mode"];
        var task = mode switch
        {
            "showAddForm" => ShowAddForm(response, sessionInfo, query, localPath),
            "tryAdd" => TryAddNews(response, sessionInfo, query, localPath),
            "tryDelete" => TryRemoveNews(response, sessionInfo, query, localPath),
            "showEditForm" => ShowEditForm(response, sessionInfo, query, localPath),
            "tryEdit" => TryEditNews(response, sessionInfo, query, localPath),
            _ => ShowNewsList(response, sessionInfo, query, localPath),
        };
        await task.FastAwait();
    }

    private async Task ShowNewsList(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var db = BotController.dataBase.db;
        var allNews = await db.Table<NewsData>().OrderByDescending(x => x.id).ToArrayAsync().FastAwait();

        var document = HtmlHelper.CreateDocument("News Editor");
        var table = document["html"]["body"].Add("table", new HProp("cellpadding", "10"), new HProp("style", "border-collapse: collapse;"));
        var headers = table.Add("tr", new HProp("style", "height: 50px; border-bottom: 1pt solid black; background-color: #DDDDDD"));
        headers.Add("th", "ID", new HProp("align", "left"));
        headers.Add("th", "Date", new HProp("align", "left"));
        headers.Add("th", "Title", new HProp("align", "left"));
        headers.Add("th", "Description", new HProp("align", "left"));
        headers.Add("th", "&nbsp;");
        headers.Add("th", "&nbsp;");

        foreach (var news in allNews)
        {
            var row = table.Add("tr", new HProp("style", "height: 50px; border-bottom: 1pt solid black;"));
            row.Add("td", $"ID {news.id}");
            row.Add("td", news.date.ToShortDateString());
            row.Add("td").Add("b", news.title);
            row.Add("td", news.description.Substring(0, Math.Min(140, news.description.Length)));
            row.Add("td").Add(HtmlHelper.CreateLinkButton("Edit", localPath + $"?page={page}&mode=showEditForm&newsId={news.id}", color: "#007FFF", size: 10));
            row.Add("td").Add(HtmlHelper.CreateLinkButton("Delete", localPath + $"?page={page}&mode=tryDelete&newsId={news.id}", color: "#CC0000", size: 10));
        }

        var specialControls = new HTag("div", new HProp("style", "margin-top: 10px;"))
        {
            HtmlHelper.CreateLinkButton("Add New", localPath + $"?page={page}&mode=showAddForm", color: "#007FFF", size: 16)
        };
        document["html"]["body"].Add(specialControls);

        var bottomPanel = new HTag("div", new HProp("style", "margin-left: 300px; margin-top: 40px;"))
        {
            HtmlHelper.CreateLinkButton("<< Back", localPath)
        };
        document["html"]["body"].Add(bottomPanel);

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private Task ShowAddForm(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var document = HtmlHelper.CreateDocument("Add News");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent(), new HProp("align", "center"));

        var centerBlock = new HTag("div", StylesHelper.CenterScreenBlock(700, 900), new HProp("align", "center"))
        {
            new HtmlFormBuilder(localPath)
                .AddHeader("Create news")
                .AddHiddenInput("page", page)
                .AddHiddenInput("mode", "tryAdd")
                .AddInput("title", "Title", fullWidth: true)
                .AddTextArea("description", "Enter text here...", rows: 20, fullWidth: true)
                .AddButton("Add")
                .GetResult()
        };
        document["html"]["body"].Add(centerBlock);

        var bottomPanel = new HTag("div", new HProp("align", "center"), new HProp("style", "margin-top: 100px;"))
        {
            HtmlHelper.CreateLinkButton("<< Back", localPath)
        };
        centerBlock.Add(bottomPanel);

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
        return Task.CompletedTask;
    }

    private async Task TryAddNews(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var hasAdminRights = await CheckAdminRights(response, sessionInfo, localPath).FastAwait();
        if (!hasAdminRights)
        {
            return;
        }

        var title = query["title"]?.Trim();
        var description = query["description"]?.Trim();
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
        {
            var errorMessage = HtmlHelper.CreateMessagePage("Incorrect text", "Title and Text can not be empty", localPath + $"?page{page}");
            response.AsTextUTF8(errorMessage.GenerateHTML());
            response.Close();
            return;
        }

        var newsData = await newsService.AddNews(title, description).FastAwait();
        Program.logger.Info($"Administrator {sessionInfo.GetAdminView()} added news:" +
            $"\nnewsId: {newsData.id}" +
            $"\ntitle: {newsData.title}" +
            $"\ndescription: {newsData.description}\n");

        response.Redirect(localPath + $"?page={page}");
        response.Close();
    }

    private async Task TryRemoveNews(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var hasAdminRights = await CheckAdminRights(response, sessionInfo, localPath).FastAwait();
        if (!hasAdminRights)
        {
            return;
        }

        if (int.TryParse(query["newsId"], out var newsId))
        {
            var newsData = await newsService.TreGetNewsById(newsId).FastAwait();
            if (newsData is not null)
            {
                await newsService.RemoveNews(newsId).FastAwait();
                Program.logger.Info($"Administrator {sessionInfo.GetAdminView()} removed news:" +
                    $"\nnewsId: {newsData.id}" +
                    $"\ntitle: {newsData.title}" +
                    $"\ndescription: {newsData.description}\n");
            }
        }        

        response.Redirect(localPath + $"?page={page}");
        response.Close();
    }

    private async Task ShowEditForm(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        if (!int.TryParse(query["newsId"], out var newsId))
        {
            response.Redirect(localPath + $"?page={page}");
            response.Close();
            return;
        }

        var newsData = await newsService.TreGetNewsById(newsId).FastAwait();
        if (newsData is null)
        {
            response.Redirect(localPath + $"?page={page}");
            response.Close();
            return;
        }

        var document = HtmlHelper.CreateDocument("Edit News");
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent(), new HProp("align", "center"));

        var centerBlock = new HTag("div", StylesHelper.CenterScreenBlock(700, 900), new HProp("align", "center"))
        {
            new HtmlFormBuilder(localPath)
                .AddHeader("Edit news")
                .AddHiddenInput("page", page)
                .AddHiddenInput("mode", "tryEdit")
                .AddHiddenInput("newsId", newsId.ToString())
                .AddInput("title", "Title", defaultValue: newsData.title, fullWidth: true)
                .AddTextArea("description", "Enter text here...", defaultValue: newsData.description, rows: 20, fullWidth: true)
                .AddButton("Save")
                .GetResult()
        };
        document["html"]["body"].Add(centerBlock);

        var bottomPanel = new HTag("div", new HProp("align", "center"), new HProp("style", "margin-top: 100px;"))
        {
            HtmlHelper.CreateLinkButton("<< Back", localPath)
        };
        centerBlock.Add(bottomPanel);

        response.AsTextUTF8(document.GenerateHTML());
        response.Close();
    }

    private async Task TryEditNews(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, NameValueCollection query, string localPath)
    {
        var hasAdminRights = await CheckAdminRights(response, sessionInfo, localPath).FastAwait();
        if (!hasAdminRights)
        {
            return;
        }

        if (!int.TryParse(query["newsId"], out var newsId))
        {
            var errorMessage = HtmlHelper.CreateMessagePage("Incorrect newsId", "Incorrect newsId", localPath + $"?page{page}");
            response.AsTextUTF8(errorMessage.GenerateHTML());
            response.Close();
            return;
        }

        var title = query["title"]?.Trim();
        var description = query["description"]?.Trim();
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
        {
            var errorMessage = HtmlHelper.CreateMessagePage("Incorrect text", "Title and Text can not be empty", localPath + $"?page{page}");
            response.AsTextUTF8(errorMessage.GenerateHTML());
            response.Close();
            return;
        }

        await newsService.TryEditNews(newsId, title, description).FastAwait();
        Program.logger.Info($"Administrator {sessionInfo.GetAdminView()} edited news:" +
            $"\nnewsId: {newsId}" +
            $"\ntitle: {title}" +
            $"\ndescription: {description}\n");

        response.Redirect(localPath + $"?page={page}");
        response.Close();
    }

    private async Task<bool> CheckAdminRights(HttpListenerResponse response, HttpAdminSessionInfo sessionInfo, string localPath)
    {
        if (sessionInfo.withoutLogin)
        {
            return true;
        }

        var db = BotController.dataBase.db;
        var profileData = await db.Table<ProfileData>().Where(x => x.telegram_id == sessionInfo.telegramId).FirstOrDefaultAsync().FastAwait();
        if (profileData is null || profileData.adminStatus < AdminStatus.Admin)
        {
            var error = HtmlHelper.CreateMessagePage("Forbidden", $"You dont have admin rights", localPath);
            response.AsTextUTF8(error.GenerateHTML());
            response.Close();
            return false;
        }

        return true;
    }

}
