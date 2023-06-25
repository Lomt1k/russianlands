using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Services.News;
public class NewsService : Service
{
    public const int LAST_NEWS_COUNT = 7;

    private NewsData[] _lastNews = Array.Empty<NewsData>();

    public int lastNewsId => _lastNews.Length > 0 ? _lastNews[0].id : -1;
    public NewsData[] lastNews => _lastNews;

    public override async Task OnBotStarted()
    {
        await RefreshLastNews().FastAwait();
    }

    private async Task RefreshLastNews()
    {
        var db = BotController.dataBase.db;
        _lastNews = await db.Table<NewsData>().OrderByDescending(x => x.id).Take(LAST_NEWS_COUNT).ToArrayAsync().FastAwait();
    }

    public async Task<NewsData> AddNews(string title, string description, DateTime? date = null)
    {
        var newsData = new NewsData
        {
            date = date.HasValue ? date.Value : DateTime.UtcNow,
            title = title,
            description = description,
        };
        var db = BotController.dataBase.db;
        await db.InsertAsync(newsData).FastAwait();
        await RefreshLastNews();
        return newsData;
    }

    public async Task RemoveNews(int newsId)
    {
        var db = BotController.dataBase.db;
        await db.DeleteAsync<NewsData>(newsId).FastAwait();
        await RefreshLastNews();
    }

    public async Task<NewsData?> TreGetNewsById(int newsId)
    {
        var db = BotController.dataBase.db;
        return await db.Table<NewsData>().Where(x => x.id == newsId).FirstOrDefaultAsync().FastAwait();
    }

    public async Task<bool> TryEditNews(int newsId, string title, string description)
    {
        var db = BotController.dataBase.db;
        var newsData = await db.Table<NewsData>().Where(x => x.id == newsId).FirstOrDefaultAsync().FastAwait();
        if (newsData is null)
        {
            return false;
        }
        newsData.title = title;
        newsData.description = description;
        var result = await db.UpdateAsync(newsData).FastAwait();
        if (result > 0)
        {
            await RefreshLastNews();
        }
        return result > 0 ? true : false;
    }

    public bool HasNew(GameSession session)
    {
        return lastNewsId > session.profile.data.lastNewsId;
    }

}
